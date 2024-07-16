import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { usePlanSectionsList, usePlan } from "api/plans";
import { NotFound } from "pages/error/NotFound";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";

export const PlanOverview = () => {
  const { projectId, planId } = useParams();
  const { data: plan, mutate } = usePlan(planId);

  const { data: sections } = usePlanSectionsList(planId);

  const planSections = sections?.map((section) => ({
    ...section,
    path: `/projects/${projectId}/plans/${planId}/sections/${section.id}`,
  }));

  if (!plan) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Plan,
    header: `${plan?.title || planId}`,
    projectName: plan?.projectName,
    owner: plan?.ownerName,
    overviewTitle: "Plan Overview",
  };

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
          isPlan
        />
      }
    />
  );
};
