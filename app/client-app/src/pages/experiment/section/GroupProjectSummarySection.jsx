import { useParams } from "react-router-dom";
import { Section } from ".";
import { SECTION_TYPES } from "constants/section-types";
import {
  useProjectGroup,
  useProjectGroupSummarySection,
} from "api/projectGroups";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";

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
      href: `/projects/${projectId}`,
    },
    ...(isInstructor
      ? [
          {
            label: projectGroup?.students[0]?.name,
            href: `/projects/${projectId}/students/${projectGroup?.students[0]?.id}`,
          },
        ]
      : []),

    {
      label: projectGroup?.name,
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
