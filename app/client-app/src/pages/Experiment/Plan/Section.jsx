import { HStack, VStack, Button, Text, useToast } from "@chakra-ui/react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Formik, Form } from "formik";
import { Layout } from "components/experiment/Layout";
import { ExperimentField } from "components/experiment/section/ExperimentField";
import { EditableHeader } from "components/experiment/section/EditableHeader";
import { FaPlus } from "react-icons/fa";
import { useParams } from "react-router-dom";
import { useExperiment } from "api/experiments";
import { useSection } from "api/section";
import { INPUT_TYPES } from "constants/input-types";

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
          return [[field.id, field.fieldResponse ?? []]]; // set value as empty array if response is null

        default:
          return [[field.id, field.fieldResponse]];
      }
    })
  );

export const Section = () => {
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

  const handleSubmit = async (values) => {
    /*
    TODO: Send the field responses to the backend and process them accordingly
    try {
      setIsLoading(true);
      console.log({...values,
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
  const actionSection = (
    <HStack pb={1}>
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
    </HStack>
  );

  return (
    <Layout>
      <EditableHeader
        header={section.name}
        experiment={experiment}
        actionSection={actionSection}
      />

      <VStack
        align="stretch"
        w={{ base: "100%", md: "90%", lg: "85%", xl: "85%" }}
      >
        <Formik
          enableReinitialize
          initialValues={initialValues(section.fieldResponses)}
          innerRef={formRef}
          onSubmit={handleSubmit}
        >
          <Form noValidate>
            <VStack align="stretch" spacing={4}>
              {section.fieldResponses
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map((field) => (
                  <ExperimentField
                    key={field.id}
                    field={field}
                    experimentId={experiment.id}
                  />
                ))}
            </VStack>
          </Form>
        </Formik>
      </VStack>
    </Layout>
  );
};
