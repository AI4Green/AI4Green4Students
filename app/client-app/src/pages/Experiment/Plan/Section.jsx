import {
  HStack,
  VStack,
  Button,
  Text,
  useToast,
  Avatar,
} from "@chakra-ui/react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Formik, Form } from "formik";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { ExperimentField } from "components/experiment/section/ExperimentField";
import { Header } from "components/experiment/section/Header";
import { FaPlus } from "react-icons/fa";
import { useParams } from "react-router-dom";
import { useExperiment } from "api/experiments";
import { useSection } from "api/section";
import { INPUT_TYPES } from "constants/input-types";
import { useUser } from "contexts/User";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
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

export const Section = () => {
  const { user } = useUser();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { experimentId, sectionId } = useParams();
  const { data: experiment } = useExperiment(experimentId);
  const { data: section } = useSection(sectionId, experimentId);
  const { t } = useTranslation();
  const toast = useToast();

  useEffect(() => {
    feedback &&
      toast({
        position: "top",
        title: feedback.message,
        status: feedback.status,
        duration: 1500,
        isClosable: true,
      });
  }, [feedback]);

  const handleSubmit = async (values, fields) => {
    /*
    TODO: Send the field responses to the backend and process them accordingly
    let submissionData = {};
    try {
      setIsLoading(true);
      fields.forEach((field) =>
        evaluateFieldCondition(field, fields, values, submissionData)
      );
      console.log({...submissionData,
        sectionId,
        experimentId,
      });

      setFeedback({
        status: "success",
        message: "Section response values saved",
      });
      setIsLoading(false);
    } catch (e) {
      console.error(e);
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
    */
  };

  const formRef = useRef();
  const isInstuctor = user.permissions?.includes(
    EXPERIMENTS_PERMISSIONS.ViewAllExperiments
  );

  const actionSection = (
    <HStack pb={1}>
      <Avatar name={experiment.ownerName} size="sm" />
      <Text fontSize="md" color="gray.600">
        {experiment.ownerName}
      </Text>
      {!isInstuctor && (
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
        header={experiment.title}
        subHeader={experiment.projectName}
        overview={section.name}
        actionSection={actionSection}
      />

      <VStack
        align="stretch"
        w={{ base: "100%", md: "90%", lg: "85%", xl: "85%" }}
      >
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
                          experimentId={experiment.id}
                          isInstructor={isInstuctor}
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

const evaluateFieldCondition = (field, fields, values, submissionData) => {
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
