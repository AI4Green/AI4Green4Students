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
  const fieldType = field.fieldType.toUpperCase();

  let result = [
    {
      fieldType,
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

/**
 * Helper function to prepare the section form data for submission
 * @param {*} fields - fields collection for given section
 * @param {*} values - field values collection
 * @returns - returns the data that is ready for submission
 */
export const prepareSubmissionData = (fields, values) => {
  const { File, ImageFile } = INPUT_TYPES;

  const data = fields.map((field) => evaluateField(field, fields, values));

  const uniqueData = Object.values(
    data.reduce((uniqueItems, itemArray) => {
      itemArray?.forEach((item) => {
        item && (uniqueItems[item.fieldId] = item);
      });
      return uniqueItems;
    }, {})
  );

  const processObject = (acc, obj, isNew) => {
    const { fieldId, fieldResponseId, fieldType } = obj;
    const isFileType =
      fieldType.toUpperCase() === File.toUpperCase() ||
      fieldType.toUpperCase() === ImageFile.toUpperCase();
    const id = isNew ? fieldId : fieldResponseId;
    const filesArray = obj.value || [];

    if (isFileType) {
      acc[isNew ? "newFiles" : "files"].push(
        ...filesArray.map((f) => (f.isNew ? f.file : new Blob()))
      );

      acc[isNew ? "newFileFieldResponses" : "fileFieldResponses"].push(
        ...filesArray.map((file) => ({
          id,
          value: Object.fromEntries(
            Object.entries(file).filter(([key]) => key !== "file")
          ),
        }))
      );
    } else {
      acc[isNew ? "newFieldResponses" : "fieldResponses"].push({
        id,
        value: obj.value,
      });
    }
    return acc;
  };

  const {
    fieldResponses,
    newFieldResponses,
    files,
    fileFieldResponses,
    newFiles,
    newFileFieldResponses,
  } = uniqueData.reduce(
    (acc, obj) => {
      return processObject(acc, obj, !obj.fieldResponseId);
    },
    {
      fieldResponses: [],
      newFieldResponses: [],
      files: [],
      fileFieldResponses: [],
      newFiles: [],
      newFileFieldResponses: [],
    }
  );

  return {
    fieldResponses,
    newFieldResponses,
    files,
    fileFieldResponses,
    newFiles,
    newFileFieldResponses,
  };
};
