import { usePlan, usePlanSectionsList, useProjectGroup } from "api";
import {
  InstructorActions,
  StudentActions,
} from "components/experiment-summary";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useUser } from "contexts";
import { NotFound } from "pages/error";
import { useParams } from "react-router-dom";
import {
  buildProjectPath,
  buildSectionFormPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

import { Overview } from "./overview";

export const PlanOverview = () => {
  const { projectId, projectGroupId, planId } = useParams();

  const { user } = useUser();
  const { data: plan, mutate } = usePlan(planId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = plan?.owner.id === user.userId;

  const { data: sections } = usePlanSectionsList(planId);
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );

  if (!plan) return <NotFound />;

  const planSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.Plan,
      projectId,
      projectGroupId,
      planId,
      section.id
    ),
  }));

  const headerItems = {
    header: {
      type: "Plan",
      icon: TITLE_ICON_COMPONENTS.Plan,
      title: plan?.title,
    },
    project: plan?.project,
    owner: plan?.owner,
    action: isInstructor ? (
      <InstructorActions
        record={{ ...plan, mutate }}
        isEverySectionApproved={sections?.every(
          (section) => section.feedback.approved
        )}
        sectionType={SECTION_TYPES.Plan}
        sections={sections}
      />
    ) : (
      <StudentActions
        record={{ ...plan }}
        sectionType={SECTION_TYPES.Plan}
        sections={sections}
      />
    ),
  };

  const breadcrumbItems = [
    { label: "Projects", href: "/projects" },
    {
      label: plan?.project.name,
      href: buildProjectPath(projectId),
    },
    ...(!isOwner
      ? [
          {
            label: projectGroup.name,
            href:
              !isInstructor &&
              buildStudentsProjectGroupPath(projectId, projectGroup?.id),
          },
          {
            label: plan?.owner.name,
            href: buildProjectPath(projectId, projectGroup?.id, plan?.owner.id),
          },
        ]
      : []),
    {
      label: plan?.title,
    },
  ];

  const item = {
    id: plan.id,
    type: SECTION_TYPES.Plan,
    stage: {
      name: plan?.stage,
      permissions: plan?.permissions,
    },
    isOwner,
  };

  return (
    <Overview
      item={item}
      sections={planSections}
      headerItems={headerItems}
      breadcrumbs={breadcrumbItems}
      isInstructor={isInstructor}
    />
  );
};
