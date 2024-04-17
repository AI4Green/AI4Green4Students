import { INPUT_TYPES } from "constants/input-types";
import { array, mixed, object, string } from "yup";
import { isTriggered } from "./fieldEvaluation";

/**
 * Create base validator based on the field type
 * @param {*} fieldType - type of the field
 * @returns
 */
const createBaseValidator = (fieldType) => {
  const { Text, Description, Multiple, Radio, ImageFile } = INPUT_TYPES;

  switch (fieldType.toUpperCase()) {
    case Text.toUpperCase():
    case Description.toUpperCase():
      return string().required("This field is required");

    case Multiple.toUpperCase():
    case Radio.toUpperCase():
      return array()
        .min(1, "Please select at least one option")
        .required("This field is required");

    case ImageFile.toUpperCase():
      return array()
        .of(
          object().shape({
            file: mixed().when("isNew", {
              is: true,
              then: mixed().required("File is required"),
              otherwise: mixed().notRequired(),
            }),
            caption: string().when("isMarkedForDeletion", {
              is: true,
              then: string().notRequired(),
              otherwise: string().required("Caption is required"),
            }),
          })
        )
        .min(1, "Please upload at least one image")
        .test(
          "all-marked-for-deletion",
          "Please upload at least one image.",
          (items) => {
            return !items.every((item) => item.isMarkedForDeletion);
          }
        );

    default:
      return mixed().notRequired(); // no validation required for other field types
  }
};

/**
 * Create validation schema for top level fields
 * @param {*} field - field to create schema for
 * @returns
 */
const createTopLevelFieldSchema = (field) => {
  const baseValidator = createBaseValidator(field.fieldType);
  return [field.id, baseValidator];
};

/**
 * Create validation schema for nested fields
 * @param {*} field - field to create schema for
 * @param {*} allFields - fields collection to search for parent
 * @returns
 */
const createNestedOrTriggeredFieldSchema = (field, allFields) => {
  const parentField = allFields.find((x) => x.trigger?.target === field.id);
  if (!parentField) return null; // return null if the field has no parent

  let baseValidator = mixed();

  const validator = baseValidator.when(`"${parentField?.id}"`, {
    is: (parentValue) =>
      isTriggered(
        parentField.fieldType,
        parentField.trigger.value,
        parentValue
      ) ?? false, // check if the field is triggered by their parent field
    then: (schema) => {
      baseValidator = createBaseValidator(field.fieldType);
      return baseValidator.required("This field is required"); // return required validator if the field is triggered
    },
    otherwise: baseValidator.notRequired(),
  });

  return [field.id, validator];
};

/**
 * Create validation schema for the fields collection
 * @param {*} fields - fields collection
 * @returns
 */
export const validationSchema = (fields) => {
  const topLevelFields = fields.filter(
    (field) => !field.hidden && field.mandatory // assuming hidden fields as not top level
  );
  const nestedOrTriggeredFields = fields.filter(
    (field) => field.hidden && field.mandatory
  );

  const schemaFields = [
    ...topLevelFields.map((field) => createTopLevelFieldSchema(field)),
    ...nestedOrTriggeredFields
      .map((field) => createNestedOrTriggeredFieldSchema(field, fields))
      .filter(Boolean),
  ];

  return object().shape(Object.fromEntries(schemaFields));
};
