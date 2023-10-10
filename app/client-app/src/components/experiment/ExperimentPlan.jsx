import { VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { DescriptionTextArea } from "components/forms/DescriptionTextArea";
import { FileUpload as LiteratureReviewFileUpload } from "components/forms/FileUpload";
import { fetchKeys } from "api/experiments";
import { ReferenceField } from "./ReferenceField";

export const ExperimentPlan = ({ experiment, formRef, onSubmit }) => {
  const { downloadFile } = fetchKeys;
  return (
    <VStack
      align="stretch"
      w={{ base: "100%", md: "90%", lg: "85%", xl: "85%" }}
    >
      <Formik
        enableReinitialize
        innerRef={formRef}
        initialValues={{
          title: experiment?.title,
          literatureReviewDescription: experiment?.literatureReviewDescription,
          literatureReviewFile: null,
          safetyDataFromLiterature: experiment?.safetyDataFromLiterature,
          experimentalProcedure: experiment?.experimentalProcedure,
          isLiteratureReviewFilePresent: !!experiment?.literatureFileName,
          references: experiment?.references ?? [],
        }}
        onSubmit={onSubmit}
      >
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
            <FormikInput name="title" label="Experiment title" isRequired />
            <DescriptionTextArea
              name="literatureReviewDescription"
              title="Literature review"
              placeholder="Literature review"
            />
            <DescriptionTextArea
              name="safetyDataFromLiterature"
              title="Safety data from literature (including toxicity)"
              placeholder="Safety data from literature (including toxicity)"
            />
            <DescriptionTextArea
              name="experimentalProcedure"
              title="Experimental procedure materials and steps"
              placeholder="Experimental procedure materials and steps"
            />
            <LiteratureReviewFileUpload
              name="literatureReviewFile"
              isFilePresentName="isLiteratureReviewFilePresent"
              title="Upload File"
              accept={["pdf", "docx", "doc"]}
              existingFile={experiment?.literatureFileName}
              downloadLink={downloadFile(
                experiment?.id,
                experiment?.literatureFileName
              )}
            />
            <ReferenceField label="References" name="references" />
          </VStack>
        </Form>
      </Formik>
    </VStack>
  );
};
