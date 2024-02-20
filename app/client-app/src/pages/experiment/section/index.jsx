import { useRef, useState } from "react";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { Header } from "components/experiment/section/Header";
import { SectionFormContext } from "contexts/SectionForm";
import { SectionFormAction } from "components/experiment/section/form/SectionFormAction";
import { SectionForm } from "components/experiment/section/form/SectionForm";
import { useBackendApi } from "contexts/BackendApi";
import { useTranslation } from "react-i18next";
import { useToast } from "@chakra-ui/react";
import { prepareSubmissionData } from "components/experiment/section/form/fieldEvaluation";

export const Section = ({ record, section, mutate, sectionType }) => {
  const [isLoading, setIsLoading] = useState();
  const { sections: actions } = useBackendApi();
  const { t } = useTranslation();
  const toast = useToast();
  const formRef = useRef();

  const handleSubmit = async (values, fields) => {
    const fieldResponses = prepareSubmissionData(fields, values);
    const payload = {
      fieldResponses: JSON.stringify(fieldResponses),
      sectionId: section.id, // id of the section.
      recordId: record.id, // id of the record i.e. plan, literature review, etc
      sectionType,
    };

    // convert the payload to FormData
    const form = new FormData();

    for (const [k, v] of Object.entries(payload)) {
      if (Array.isArray(v)) {
        for (let i = 0; i < v.length; i++) {
          form.append(`${k}[]`, v[i]);
        }
      } else if (typeof v === "object" && v !== null) {
        console.log(k);
        form.append(k, JSON.stringify(v));
      } else {
        form.append(k, v);
      }
    }

    try {
      setIsLoading(true);
      const response = await actions.saveFieldResponses(form);
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
      }}
    >
      <ExperimentLayout>
        <Header
          header={record?.title ?? record.id}
          subHeader={record.projectName}
          overview={section.name}
          actionSection={
            <SectionFormAction
              record={record}
              isLoading={isLoading}
              formRef={formRef}
            />
          }
        />

        <SectionForm
          sectionFields={section.fieldResponses}
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
