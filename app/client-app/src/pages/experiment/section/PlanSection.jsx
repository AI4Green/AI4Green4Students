import { useParams } from "react-router-dom";
import { usePlan, usePlanSection, useProjectGroup } from "api";
import { Section } from "./Section";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi, useUser } from "contexts";
import { useIsInstructor } from "helpers/hooks";
import {
  buildOverviewPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";

export const PlanSection = () => {
  const { user } = useUser();
  const { planId, projectId, projectGroupId, sectionId } = useParams();
  const { data: plan } = usePlan(planId);
  const { data: planSection, mutate } = usePlanSection(planId, sectionId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { plans } = useBackendApi();

  const isInstructor = useIsInstructor();
  const isAuthor = plan?.ownerId === user.userId;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Plan,
    header: plan?.title,
    projectName: plan?.projectName,
    owner: plan?.ownerName,
    overviewTitle: planSection?.name?.toLowerCase().endsWith("form")
      ? planSection?.name
      : `${planSection?.name} Form`,
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
      href: buildOverviewPath(
        SECTION_TYPES.Plan,
        projectId,
        projectGroup?.id,
        plan?.id
      ),
    },
    {
      label: planSection?.name,
    },
  ];

  return (
    <Section
      record={plan}
      section={planSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Plan}
      headerItems={headerItems}
      save={plans.saveFieldResponses}
      breadcrumbItems={breadcrumbItems}
    />
  );
};
