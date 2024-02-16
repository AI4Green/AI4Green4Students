import { useParams } from "react-router-dom";
import { usePlan } from "api/plans";
import { usePlanSection } from "api/section";
import { Section } from ".";
import { SECTION_TYPES } from "constants/section-types";

export const PlanSection = () => {
  const { planId, sectionId } = useParams();
  const { data: plan } = usePlan(planId);
  const { data: planSection, mutate } = usePlanSection(planId, sectionId);

  return (
    <Section
      record={plan}
      section={planSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Plan}
    />
  );
};
