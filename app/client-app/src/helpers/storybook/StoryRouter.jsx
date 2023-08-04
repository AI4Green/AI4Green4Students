import { unstable_HistoryRouter as HistoryRouter } from "react-router-dom";
import { createMemoryHistory } from "history";
import { action } from "@storybook/addon-actions";

const history = createMemoryHistory();
history.push = action("history.push");
history.replace = action("history.replace");
history.go = action("history.go");
history.goBack = action("history.goBack");
history.goForward = action("history.goForward");

export const StoryRouter = ({ children }) => (
  <HistoryRouter history={history}>{children}</HistoryRouter>
);
