import { Overview } from "./Overview";
import { useParams } from "react-router-dom";
import { usePlanSectionsList, usePlan, useProjectGroup } from "api";
import { NotFound } from "pages/error";
import { Breadcrumbs } from "components/core/Breadcrumbs";
import { TITLE_ICON_COMPONENTS, SECTION_TYPES } from "constants";
import { useIsInstructor } from "helpers/hooks";
import {
  buildSectionFormPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts";
import { InstructorAction, StudentAction } from "components/experiment-summary";

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

  const isInstructor = useIsInstructor();
  const isAuthor = plan?.ownerId === user.userId;

  if (!plan) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Plan,
    header: plan?.title,
    projectName: plan?.projectName,
    owner: plan?.ownerName,
    ownerId: plan?.ownerId,
    overviewTitle: "Plan Overview",
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: plan?.projectName,
      href: buildProjectPath(projectId),
    },
    ...(!isAuthor
      ? [
          {
            label: projectGroup.name,
            href:
              !isInstructor &&
              buildStudentsProjectGroupPath(projectId, projectGroup?.id),
          },
          {
            label: plan?.ownerName,
            href: buildProjectPath(projectId, projectGroup?.id, plan?.ownerId),
          },
        ]
      : []),
    {
      label: plan?.title,
    },
  ];

  return (
    <Overview
      sections={planSections}
      headerItems={headerItems}
      InstructorAction={
        <InstructorAction
          record={{ ...plan, mutate }}
          isEverySectionApproved={sections?.every(
            (section) => section.approved
          )}
          sectionType={SECTION_TYPES.Plan}
          sections={sections}
        />
      }
      StudentAction={
        <StudentAction
          record={{ ...plan }}
          sectionType={SECTION_TYPES.Plan}
          sections={sections}
        />
      }
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
