import { HStack, VStack, Button, Text, Avatar } from "@chakra-ui/react";
import { useRef } from "react";
import { Formik, Form } from "formik";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { ExperimentField } from "components/experiment/section/ExperimentField";
import { Header } from "components/experiment/section/Header";
import { FaPlus } from "react-icons/fa";
import { INPUT_TYPES } from "constants/input-types";
import { object, string, array, mixed } from "yup";

const initialValues = (section) =>
  // creates an object with the field.id as key and field.response as value.
  Object.fromEntries(
    section.flatMap((field) => {
      switch (field.fieldType.toUpperCase()) {
        case INPUT_TYPES.File.toUpperCase():
          return [
            [field.id, field.fieldResponse],
            // concatenating field.id with '_isFilePresent' to append a new bool property and set its value as 'true' if response has a fileName
            [`${field.id}_isFilePresent`, !!field.fieldResponse?.fileName],
          ];

        case INPUT_TYPES.Text.toUpperCase():
        case INPUT_TYPES.Description.toUpperCase():
          return [[field.id, field.fieldResponse ?? ""]]; // set value as empty string if response is null

        case INPUT_TYPES.Multiple.toUpperCase():
        case INPUT_TYPES.Radio.toUpperCase():
        case INPUT_TYPES.DraggableList.toUpperCase():
        case INPUT_TYPES.ChemicalDisposalTable.toUpperCase():
          return [[field.id, field.fieldResponse ?? []]]; // set value as empty array if response is null

        case INPUT_TYPES.Header.toUpperCase():
        case INPUT_TYPES.Content.toUpperCase():
          return [];

        default:
          return [[field.id, field.fieldResponse]];
      }
    })
  );

const createBaseValidator = (fieldType) => {
  let baseValidator;
  switch (fieldType.toUpperCase()) {
    case INPUT_TYPES.Text.toUpperCase():
    case INPUT_TYPES.Description.toUpperCase():
      baseValidator = string();
      break;
    case INPUT_TYPES.Multiple.toUpperCase():
    case INPUT_TYPES.Radio.toUpperCase():
      baseValidator = array().min(1, "Please select at least one option");
      break;
    default:
      return mixed().notRequired(); // no validation required for other field types
  }
  return baseValidator.required("This field is required");
};

const validationSchema = (fields) => {
  let schemaFields = {};

  const topLevelFields = fields.filter(
    (field) => !field.hidden && field.mandatory
  );
  const nestedOrTriggeredFields = fields.filter(
    (field) => field.hidden && field.mandatory
  );

  topLevelFields.forEach((field) => {
    const baseValidator = createBaseValidator(field.fieldType);
    schemaFields[field.id] = baseValidator;
  });

  nestedOrTriggeredFields.forEach((field) => {
    const parentField = fields.find((x) => x.trigger?.target === field.id);
    if (!parentField) return;

    let baseValidator = mixed();

    schemaFields[field.id] = baseValidator.when(`"${parentField?.id}"`, {
      is: (parentValue) => isTriggered(parentField, parentValue) ?? false,
      then: (schema) => {
        baseValidator = createBaseValidator(field.fieldType);
        return baseValidator.required("This field is required");
      },
      otherwise: baseValidator.notRequired(),
    });
  });

  return object().shape(schemaFields);
};

export const Section = ({
  isInstructor,
  record,
  isLoading,
  section,
  handleSubmit,
}) => {
  const formRef = useRef();

  const actionSection = (
    <HStack pb={1}>
      <Avatar name={record.ownerName} size="sm" />
      <Text fontSize="md" color="gray.600">
        {record.ownerName}
      </Text>
      {!isInstructor && (
        <Button
          colorScheme="green"
          leftIcon={<FaPlus />}
          size="xs"
          isLoading={isLoading}
          onClick={() => formRef.current.handleSubmit()}
        >
          <Text fontSize="xs" fontWeight="semibold">
            Save
          </Text>
        </Button>
      )}
    </HStack>
  );

  return (
    <ExperimentLayout>
      <Header
        header={record?.title ?? record.id}
        subHeader={record.projectName}
        overview={section.name}
        actionSection={actionSection}
      />

      <VStack align="stretch" w="full">
        <Formik
          enableReinitialize
          initialValues={initialValues(section.fieldResponses)}
          validationSchema={validationSchema(section.fieldResponses)}
          innerRef={formRef}
          onSubmit={async (values) =>
            await handleSubmit(values, section.fieldResponses)
          }
        >
          {({ values }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                {section.fieldResponses
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map(
                    (field) =>
                      !field.hidden && (
                        <ExperimentField
                          key={field.id}
                          fieldValues={values} // values is an collection of formik values, which can be accessed by using the field.id as key
                          field={field}
                          recordId={record.id}
                          isInstructor={isInstructor}
                          sectionFields={section.fieldResponses}
                        />
                      )
                  )}
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </ExperimentLayout>
  );
};

const isTriggered = (field, parentFieldValue) => {
  // helper function determining if the trigger condition is met
  switch (field.fieldType.toUpperCase()) {
    case INPUT_TYPES.Text.toUpperCase():
    case INPUT_TYPES.Description.toUpperCase():
      return field.trigger.value === parentFieldValue;

    case INPUT_TYPES.Multiple.toUpperCase():
    case INPUT_TYPES.Radio.toUpperCase():
      return parentFieldValue.some(
        (value) => field.trigger.value === value.name
      );

    default:
      return false;
  }
};

export const evaluateFieldCondition = (
  field,
  fields,
  values,
  submissionData
) => {
  submissionData[field.id] = values[field.id];

  if (!field.trigger) {
    return; // no trigger condition, return
  }

  // find any fields triggered by the current field and evaluate their trigger conditions
  fields
    .filter((childField) => childField.id === field.trigger.target)
    .forEach((childField) => {
      const triggered = isTriggered(field, values[field.id]);

      submissionData[childField.id] = triggered ? values[childField.id] : null;

      // recursively evaluate for any further nested triggers
      if (triggered) {
        evaluateFieldCondition(childField, fields, values, submissionData);
      }
    });
};
