import { VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { initialValues } from "./initialValues";
import { validationSchema } from "./validationSchema";
import { ExperimentField } from "../field/ExperimentField";

export const SectionForm = ({ section, record, formRef, handleSubmit }) => {
  const { fieldResponses: sectionFields } = section;
  return (
    <Formik
      enableReinitialize
      initialValues={initialValues(sectionFields, record.id, section.id)}
      validationSchema={validationSchema(sectionFields)}
      innerRef={formRef}
      onSubmit={async (values) => await handleSubmit(values, sectionFields)}
    >
      {({ values }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={[2, 4]}>
            {sectionFields
              .sort((a, b) => a.sortOrder - b.sortOrder)
              .map(
                (field) =>
                  !field.hidden && (
                    <ExperimentField
                      key={field.id}
                      fieldValues={values} // values is an collection of formik values, which can be accessed by using the field.id as key
                      field={field}
                      recordId={record.id}
                      sectionFields={sectionFields}
                    />
                  )
              )}
          </VStack>
        </Form>
      )}
    </Formik>
  );
};
