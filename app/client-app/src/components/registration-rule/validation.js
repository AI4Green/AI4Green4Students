import { object, string } from "yup";

export const validation = (rules) =>
  object().shape({
    value: string()
      .notOneOf(
        rules.map((rule) => rule.value),
        "Rule value already exist"
      )
      .required("Valid value required")
      // regex to ensures that input value has only one '@' char and has at least one char after it
      .matches(/^[^@]*@[^@]+$/, "Invalid value"),
  });
