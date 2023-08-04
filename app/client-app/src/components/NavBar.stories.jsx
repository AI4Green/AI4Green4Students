import { action } from "@storybook/addon-actions";
import { BackendApiProvider } from "contexts/BackendApi";
import { NavBar } from "./NavBar";

export default {
  component: NavBar,
  decorators: [
    (Story) => (
      <BackendApiProvider
        value={{
          logout: action("Logged out"),
        }}
      >
        <Story />
      </BackendApiProvider>
    ),
  ],
};

export const Basic = {};
