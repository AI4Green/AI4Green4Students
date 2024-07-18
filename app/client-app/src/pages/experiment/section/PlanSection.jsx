import { useParams } from "react-router-dom";
import { usePlan, usePlanSection } from "api/plans";
import { Section } from ".";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";

export const PlanSection = () => {
  const { planId, projectId, sectionId } = useParams();
  const { data: plan } = usePlan(planId);
  const { data: planSection, mutate } = usePlanSection(planId, sectionId);
  const { plans } = useBackendApi();

  const isInstructor = useIsInstructor();

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Plan,
    header: `${plan?.title || planId}`,
    projectName: plan?.projectName,
    owner: plan?.ownerName,
    overviewTitle: `${planSection?.name} Form`,
  };
  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: plan?.projectName,
      href: `/projects/${projectId}`,
    },
    ...(isInstructor
      ? [
          {
            label: plan?.ownerName,
            href: `/projects/${projectId}/students/${plan?.ownerId}`,
          },
        ]
      : []),

    {
      label: plan?.title,
      href: `/projects/${projectId}/plans/${planId}/overview`,
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
