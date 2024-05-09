import { useParams } from "react-router-dom";
import { usePlan, usePlanSection } from "api/plans";
import { Section } from ".";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";

export const PlanSection = () => {
  const { planId, sectionId } = useParams();
  const { data: plan } = usePlan(planId);
  const { data: planSection, mutate } = usePlanSection(planId, sectionId);
  const { plans } = useBackendApi();

  const headerItems = {
    header: `Plan - ${plan?.title ?? planId}`,
    subHeader: plan?.projectName,
    overviewTitle: planSection?.name,
  };

  return (
    <Section
      record={plan}
      section={planSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Plan}
      headerItems={headerItems}
      save={plans.saveFieldResponses}
    />
  );
};
