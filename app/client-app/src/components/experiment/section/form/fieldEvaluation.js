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
  if (!values[field.id]) return;
  const { File } = INPUT_TYPES;
  const fieldType = field.fieldType.toUpperCase();

  let result =
    fieldType === File.toUpperCase()
      ? [
          {
            fieldId: field.id,
            fieldResponseId: field.fieldResponseId,
            value: values[field.id],
            isFilePresent: values[`${field.id}_isFilePresent`],
          },
        ]
      : [
          {
            fieldId: field.id,
            fieldResponseId: field.fieldResponseId,
            value: values[field.id],
          },
        ];

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

    if (triggered) {
      const childResult = evaluateField(childField, fields, values);
      if (childResult) {
        // Concatenate childResult into the result array
        result.push(
          ...(Array.isArray(childResult) ? childResult : [childResult])
        );
      }
    }
  });

  return result;
};

export const prepareSubmissionData = (fields, values) => {
  const data = fields.map((field) => evaluateField(field, fields, values));
  const uniqueData = Object.values(
    data.reduce((uniqueItems, itemArray) => {
      if (itemArray) {
        itemArray.forEach((item) => {
          if (item) {
            uniqueItems[item.fieldId] = item;
          }
        });
      }
      return uniqueItems;
    }, {})
  );

  const { fieldResponses, newFieldResponses } = uniqueData.reduce(
    (acc, obj) => {
      if (obj.fieldResponseId) {
        acc.fieldResponses.push({ id: obj.fieldResponseId, value: obj.value });
      } else {
        acc.newFieldResponses.push({ id: obj.fieldId, value: obj.value });
      }
      return acc;
    },
    { fieldResponses: [], newFieldResponses: [] }
  );

  return {
    fieldResponses,
    newFieldResponses,
  };
};
