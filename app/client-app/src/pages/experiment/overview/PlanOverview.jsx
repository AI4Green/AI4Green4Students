import { Overview } from ".";
import { useParams } from "react-router-dom";
import { usePlanSectionsList } from "api/section";
import { usePlan } from "api/plans";
import { NotFound } from "pages/error/NotFound";

export const PlanOverview = () => {
  const { planId, sectionTypeId } = useParams();
  const { data: plan } = usePlan(planId);

  const { data: sections } = usePlanSectionsList(planId, sectionTypeId);

  const planSections = sections?.map((section) => ({
    ...section,
    path: `/project/${section.sectionType?.name}-section/${planId}/${section.id}`,
  }));

  if (!plan) return <NotFound />;

  const headerItems = {
    header: `Plan - ${plan?.title ?? planId}`,
    subHeader: plan?.projectName,
    owner: plan?.ownerName,
    overviewTitle: "Plan Overview",
  };

  return <Overview sections={planSections} headerItems={headerItems} />;
};
