import { INPUT_TYPES } from "constants/input-types";

/**
 * Helper function to determine whether the field has triggered the child field
 * @param {*} field - field to check
 * @param {*} parentFieldValue - value of the field to check against field trigger value
 * @returns
 */
export const isTriggered = (fieldType, fieldTriggerValue, parentFieldValue) => {
  // helper function determining if the trigger condition is met

  const { Text, Description, Multiple, Radio } = INPUT_TYPES;
  switch (fieldType.toUpperCase()) {
    case Text.toUpperCase():
    case Description.toUpperCase():
      return fieldTriggerValue === parentFieldValue;

    case Multiple.toUpperCase():
    case Radio.toUpperCase():
      return parentFieldValue.some((value) => fieldTriggerValue === value.name);

    default:
      return false;
  }
};

/**
 * Evaluate field and return the result
 * @param {*} field - field to evaluate
 * @param {*} fields - fields collection
 * @param {*} values - field values collection
 * @returns - returns the evaluated field result
 */
const evaluateField = (field, fields, values) => {
  let result = { [field.id]: values[field.id] };

  if (!field.trigger) return result;

  const triggeredFields = fields.filter(
    (childField) => childField.id === field.trigger.target
  );

  triggeredFields.forEach((childField) => {
    const triggered = isTriggered(
      field.fieldType,
      field.trigger.value,
      values[field.id]
    );
    result[childField.id] = triggered ? values[childField.id] : null;

    if (triggered) {
      result = {
        ...result,
        ...evaluateField(childField, fields, values),
      };
    }
  });

  return result;
};

export const prepareSubmissionData = (fields, values) => {
  const data = fields.map((field) => evaluateField(field, fields, values));
  return Object.assign({}, ...data);
};
