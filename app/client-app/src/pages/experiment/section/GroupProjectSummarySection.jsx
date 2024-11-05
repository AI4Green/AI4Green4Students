import { useParams } from "react-router-dom";
import { Section } from "./Section";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { useProjectGroup, useProjectGroupSummarySection } from "api";
import { useBackendApi } from "contexts";
import {
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useIsInstructor } from "helpers/hooks";

export const GroupProjectSummarySection = () => {
  const { projectGroupId, projectId } = useParams();
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { data: pgSection, mutate } =
    useProjectGroupSummarySection(projectGroupId);
  const { projectGroups } = useBackendApi();
  const isInstructor = useIsInstructor();

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.ProjectGroup,
    header: `Project Group - ${projectGroup?.name || projectGroupId}`,
    projectName: projectGroup?.projectName,
    overviewTitle: "Project Group Summary",
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: projectGroup?.projectName,
      href: buildProjectPath(projectId),
    },
    {
      label: projectGroup?.name,
      href:
        !isInstructor &&
        buildStudentsProjectGroupPath(projectId, projectGroup?.id),
    },
    {
      label: "Project Group Activities",
    },
  ];

  return (
    <Section
      record={projectGroup}
      section={pgSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.ProjectGroup}
      headerItems={headerItems}
      save={projectGroups.saveFieldResponses}
      breadcrumbItems={breadcrumbItems}
    />
  );
};
