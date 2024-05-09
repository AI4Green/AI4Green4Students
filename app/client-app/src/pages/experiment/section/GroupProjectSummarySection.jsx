import { useParams } from "react-router-dom";
import { Section } from ".";
import { SECTION_TYPES } from "constants/section-types";
import {
  useProjectGroup,
  useProjectGroupSummarySection,
} from "api/projectGroups";
import { useBackendApi } from "contexts/BackendApi";

export const GroupProjectSummarySection = () => {
  const { projectGroupId, sectionTypeId } = useParams();
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { data: pgSection, mutate } = useProjectGroupSummarySection(
    projectGroupId,
    sectionTypeId
  );
  const { projectGroups } = useBackendApi();

  const headerItems = {
    header: `Project Group - ${projectGroup?.name ?? projectGroupId}`,
    subHeader: projectGroup?.projectName,
    overviewTitle: "Project Group Summary",
  };

  return (
    <Section
      record={projectGroup}
      section={pgSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.ProjectGroup}
      headerItems={headerItems}
      save={projectGroups.saveFieldResponses}
    />
  );
};
