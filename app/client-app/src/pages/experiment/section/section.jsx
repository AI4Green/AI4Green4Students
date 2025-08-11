import { SectionFormContext, useUser } from "contexts";
import { SectionForm } from "components/section-form";
import { SECTION_TYPES } from "constants";
import { useParams } from "react-router-dom";
import { useProject } from "api";

export const Section = ({
  record,
  section,
  mutate,
  sectionType,
  headerItems,
  save,
  breadcrumbItems,
}) => {
  const { projectId } = useParams();
  const { user } = useUser();
  const { data: project } = useProject(projectId);

  return (
    <SectionFormContext.Provider
      value={{
        mutate,
        isRecordOwner:
          sectionType === SECTION_TYPES.Note
            ? record?.plan?.ownerId === user.userId
            : record?.ownerId === user.userId,
        stagePermissions: record.permissions ?? [],
        stage: record.stage ?? "",
        sectionType,
        project,
        projectGroup:
          sectionType.toUpperCase() ===
            SECTION_TYPES.ProjectGroup.toUpperCase() && record,
        save,
      }}
    >
      <SectionForm
        section={section}
        record={record}
        headerItems={headerItems}
        breadcrumbItems={breadcrumbItems}
      />
    </SectionFormContext.Provider>
  );
};
