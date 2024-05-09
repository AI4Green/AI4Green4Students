import { useRef, useState } from "react";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { Header } from "components/experiment/section/Header";
import { SectionFormContext } from "contexts/SectionForm";
import { SectionFormAction } from "components/experiment/section/form/SectionFormAction";
import { SectionForm } from "components/experiment/section/form/SectionForm";
import { useTranslation } from "react-i18next";
import { useToast } from "@chakra-ui/react";
import { prepareSubmissionData } from "components/experiment/section/form/fieldEvaluation";
import { SECTION_TYPES } from "constants/section-types";

export const Section = ({
  record,
  section,
  mutate,
  sectionType,
  headerItems,
  save,
}) => {
  const [isLoading, setIsLoading] = useState();
  const { t } = useTranslation();
  const toast = useToast();
  const formRef = useRef();

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
    <SectionFormContext.Provider
      value={{
        mutate,
        stagePermissions: record.permissions ?? [],
        stage: record.stage ?? "",
        sectionType,
        projectGroup:
          sectionType.toUpperCase() ===
            SECTION_TYPES.ProjectGroup.toUpperCase() && record,
      }}
    >
      <ExperimentLayout>
        <Header
          {...headerItems}
          actionSection={
            <SectionFormAction
              record={record}
              isLoading={isLoading}
              formRef={formRef}
            />
          }
        />

        <SectionForm
          section={section}
          record={record}
          formRef={formRef}
          handleSubmit={handleSubmit}
        />
      </ExperimentLayout>
    </SectionFormContext.Provider>
  );
};

const toastOptions = (title, status) => ({
  position: "top",
  title,
  status,
  duration: 1500,
  isClosable: true,
});
