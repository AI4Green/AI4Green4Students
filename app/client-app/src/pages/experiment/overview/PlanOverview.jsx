import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { usePlanSectionsList, usePlan } from "api/plans";
import { NotFound } from "pages/error/NotFound";
import { Breadcrumbs } from "components/Breadcrumbs";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { SECTION_TYPES } from "constants/section-types";
import { buildSectionFormPath, buildProjectPath } from "routes/Project";

export const PlanOverview = () => {
  const { projectId, planId } = useParams();
  const { data: plan, mutate } = usePlan(planId);

  const { data: sections } = usePlanSectionsList(planId);

  const planSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.Plan,
      projectId,
      planId,
      section.id
    ),
  }));

  const isInstructor = useIsInstructor();

  if (!plan) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Plan,
    header: `${plan?.title || planId}`,
    projectName: plan?.projectName,
    owner: plan?.ownerName,
    overviewTitle: "Plan Overview",
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: plan?.projectName,
      href: buildProjectPath(projectId),
    },
    ...(isInstructor
      ? [
          {
            label: plan?.ownerName,
            href: buildProjectPath(projectId, isInstructor, plan?.ownerId),
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
        />
      }
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
