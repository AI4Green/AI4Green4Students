import { useRef } from "react";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { Header } from "components/experiment/section/Header";
import { SectionFormContext } from "contexts/SectionForm";
import { SectionFormAction } from "components/experiment/section/form/SectionFormAction";
import { SectionForm } from "components/experiment/section/form/SectionForm";

export const Section = ({
  record,
  isLoading,
  section,
  mutate,
  handleSubmit,
}) => {
  const formRef = useRef();
  return (
    <SectionFormContext.Provider
      value={{
        mutate,
        stagePermissions: record.permissions,
        stage: record.stage,
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
