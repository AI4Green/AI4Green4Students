import { findKeyByPropertyValue } from "helpers/data-structures";
import { countWords } from "helpers/strings";
import { bool, string } from "yup";

/**
 * Build a dictionary of Formik initial values from the questions as specified
 * @param {*} conditionalRadioQuestions
 * @returns
 */
export const getConditionalRadioInitialValues = (conditionalRadioQuestions) =>
  conditionalRadioQuestions.reduce(
    (a, { radioName, textareaName, options }) => {
      const [[firstRadioKey, firstRadioOptions]] = Object.entries(options);
      return {
        ...a,
        [radioName]: firstRadioOptions.value ?? firstRadioKey,
        [textareaName]: "",
      };
    },
    {}
  );
/**
 * Build a dictionary of yup validation schema from the questions as specified
 * @param {*} conditionalRadioQuestions
 * @returns
 */
export const getConditionalRadioValidationSchema = (
  conditionalRadioQuestions
) =>
  conditionalRadioQuestions.reduce(
    (a, { radioName, textareaName, options }) => {
      return {
        ...a,
        [radioName]: bool().required("Please select an option"),
        [textareaName]: string()
          .when(radioName, {
            is: (radioValue) => {
              const radioKey = findKeyByPropertyValue(
                options,
                "value",
                radioValue
              );
              const thisOption = options[radioKey];
              return (
                radioValue === (thisOption.value ?? radioKey) &&
                thisOption.question
              );
            },
            then: (schema) => schema.required("Please enter a response"),
          })
          .test({
            name: "wordLimit",
            exclusive: true,
            test: (value, context) => {
              const radioValue = context.parent[radioName];
              const radioKey = findKeyByPropertyValue(
                options,
                "value",
                radioValue
              );
              const thisOption = options[radioKey];

              // if our radio isn't the selected one, no further validation needed
              if (radioValue !== thisOption.value ?? radioKey) return true;

              // if it IS, then check the word limit
              if (thisOption?.wordLimit) {
                if (countWords(value ?? "") <= thisOption.wordLimit)
                  return true;

                throw context.createError({
                  params: { wordLimit: thisOption.wordLimit },
                });
              } else return true; // if no limit, always valid
            },
            message: ({ wordLimit }) =>
              `Please reduce this response to below the word limit of ${wordLimit}`,
          }),
      };
    },
    {}
  );
