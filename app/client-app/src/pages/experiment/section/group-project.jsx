import { useProjectGroup, useProjectGroupSummarySection } from "api";
import { SectionForm } from "components/section-form";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi, useUser } from "contexts";
import { useParams } from "react-router-dom";
import {
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

export const GroupProjectSummarySection = () => {
  const { projectGroupId, projectId } = useParams();

  const { user } = useUser();
  const { data: projectGroup } = useProjectGroup(projectGroupId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const { data: form, mutate } = useProjectGroupSummarySection(projectGroupId);
  const { projectGroups } = useBackendApi();

  const headerItems = {
    header: {
      type: "Project Group",
      icon: TITLE_ICON_COMPONENTS.ProjectGroup,
      title: projectGroup?.name || projectGroupId,
    },
    project: projectGroup?.project,
  };

  const breadcrumbItems = [
    { label: "Projects", href: "/projects" },
    {
      label: projectGroup?.project.name,
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

  const item = {
    id: projectGroup.id,
    type: SECTION_TYPES.ProjectGroup,
    action: {
      mutate,
      save: projectGroups.saveFieldResponses,
    },
  };

  return (
    <SectionForm
      item={item}
      form={form}
      headerItems={headerItems}
      breadcrumbItems={breadcrumbItems}
      isInstructor={isInstructor}
    />
  );
};
