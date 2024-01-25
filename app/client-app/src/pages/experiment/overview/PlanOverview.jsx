import { Overview } from ".";
import { useParams } from "react-router-dom";
import { usePlanSectionsList } from "api/section";
import { usePlan } from "api/plans";
import { NotFound } from "pages/error/NotFound";

export const PlanOverview = () => {
  const { planId, sectionTypeId } = useParams();
  const { data: plan } = usePlan(planId);

  const { data: sections } = usePlanSectionsList(planId, sectionTypeId);

  if (!plan) return <NotFound />;

  const headerItems = {
    header: plan?.title ?? `Plan ${planId}`,
    subHeader: plan?.projectName,
    owner: plan?.ownerName,
    overviewTitle: "Plan Overview",
  };

  return (
    <Overview
      sections={sections}
      recordId={plan?.id}
      headerItems={headerItems}
    />
  );
};
