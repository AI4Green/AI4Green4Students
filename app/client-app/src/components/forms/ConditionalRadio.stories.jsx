import { ConditionalRadio } from "./ConditionalRadio";

export default {
  component: ConditionalRadio,
};

export const Basic = {
  args: {
    question: "Would you like to give more details?",
    radioValue: "Yes",
    textareaValue: { Yes: "", No: "" },
    options: [
      { id: "Yes", question: "Please provide more details:", wordLimit: 5 },
      { id: "Maybe", question: "Please provide more maybe details:" },
      { id: "No" },
    ],
  },
};
