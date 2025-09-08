import { usePlan, usePlanSectionsList, useProjectGroup } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
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
  const { user } = useUser();
  const { projectId, projectGroupId, planId } = useParams();
  const { data: plan, mutate } = usePlan(planId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);

  const { data: sections } = usePlanSectionsList(planId);
  const planSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.Plan,
      projectId,
      projectGroup?.id,
      planId,
      section.id
    ),
  }));

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = plan?.ownerId === user.userId;

  if (!plan) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Plan,
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
        isEverySectionApproved={sections?.every((section) => section.approved)}
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
    { label: "Home", href: "/" },
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

  return (
    <Overview
      stage={plan?.stage}
      sections={planSections}
      headerItems={headerItems}
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
      isOwner={isOwner}
      isInstructor={isInstructor}
    />
  );
};
