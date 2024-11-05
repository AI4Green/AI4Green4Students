import { useToast, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { initialValues, prepareSubmissionData, SectionFormAction } from ".";
import { SectionField, validationSchema } from "components/section-field";
import { useTranslation } from "react-i18next";
import { GLOBAL_PARAMETERS } from "constants";
import { useSectionForm } from "contexts";
import { useRef, useState } from "react";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { Breadcrumbs } from "components/core/Breadcrumbs";
import { SectionHeader } from "components/section-header/SectionHeader";

export const SectionForm = ({
  section,
  record,
  headerItems,
  breadcrumbItems,
}) => {
  const { save, mutate } = useSectionForm();

  const [isLoading, setIsLoading] = useState();
  const { t } = useTranslation();
  const toast = useToast();
  const formRef = useRef();

  const { fieldResponses: sectionFields } = section;

  const handleSubmit = async (values, fields) => {
    const formData = prepareSubmissionData(fields, values);

    const payload = {
      fieldResponses: JSON.stringify(formData.fieldResponses), // used for updating existing field responses.
      newFieldResponses: JSON.stringify(formData.newFieldResponses), // used for creating new field responses.
      sectionId: section.id, // id of the section.
      recordId: record.id, // id of the record i.e. plan, literature review, etc

      // file related data
      files: formData.files,
      fileFieldResponses: JSON.stringify(formData.fileFieldResponses),

      newFiles: formData.newFiles,
      newFileFieldResponses: JSON.stringify(formData.newFileFieldResponses),
    };

    // convert the payload to FormData
    const form = new FormData();

    Object.entries(payload).forEach(([k, v]) => {
      if (k === "files" || k === "newFiles") {
        v.forEach((file) => form.append(k, file)); // append files to the form
      } else if (Array.isArray(v)) {
        v.forEach((item) => form.append(`${k}[]`, JSON.stringify(item)));
      } else if (typeof v === "object" && v !== null) {
        form.append(k, JSON.stringify(v)); // stringify objects and append to the form
      } else {
        form.append(k, v); // append the rest
      }
    });

    try {
      setIsLoading(true);
      const response = await save(form);
      if (response && (response.status === 204 || response.status === 200)) {
        toast(toastOptions("Section values saved successfully", "success"));
        await mutate();
      }
    } catch (e) {
      toast(toastOptions(t("feedback.error_title"), "error"));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbItems} />
      <SectionHeader
        {...headerItems}
        actionSection={
          <SectionFormAction isLoading={isLoading} formRef={formRef} />
        }
      />
      <Formik
        enableReinitialize
        initialValues={initialValues(sectionFields, record.id, section.id)}
        validationSchema={validationSchema(sectionFields)}
        innerRef={formRef}
        onSubmit={async (values) => await handleSubmit(values, sectionFields)}
      >
        {({ values }) => (
          <Form noValidate>
            <VStack align="stretch" spacing={[3, 4]}>
              {sectionFields
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map(
                  (field) =>
                    !field.hidden && (
                      <SectionField
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
    </DefaultContentLayout>
  );
};

const toastOptions = (title, status) => ({
  position: "top",
  title,
  status,
  duration: GLOBAL_PARAMETERS.ToastDuration,
  isClosable: true,
});
