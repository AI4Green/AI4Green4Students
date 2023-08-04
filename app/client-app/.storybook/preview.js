import { ChakraProvider } from "@chakra-ui/react";
import { StoryRouter } from "helpers/storybook/StoryRouter";
import { theme } from "themes/theme";

export const parameters = {
  actions: { argTypesRegex: "^on[A-Z].*" },
  controls: {
    matchers: {
      color: /(background|color)$/i,
      date: /Date$/,
    },
  },
};

export const decorators = [
  (story) => {
    return (
      <ChakraProvider theme={theme}>
        <StoryRouter history={history}>{story()}</StoryRouter>
      </ChakraProvider>
    );
  },
];
