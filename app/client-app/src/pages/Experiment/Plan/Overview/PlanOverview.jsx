import { Overview } from ".";
import { useParams } from "react-router-dom";
import { useSectionsList } from "api/section";
import { useExperiment } from "api/experiments";
import { NotFound } from "pages/error/NotFound";

export const PlanOverview = () => {
  const { experimentId } = useParams();
  const { data: experiment } = useExperiment(experimentId);

  // TODO: currently, all sections are returned. only get non-reaction sections.
  const { data: sections } = useSectionsList(experimentId);

  if (!experiment) return <NotFound />;

  const headerItems = {
    header: experiment?.title,
    subHeader: experiment?.projectName,
    owner: experiment?.ownerName,
    overviewTitle: "Experiment Plan Overview",
  };

  return (
    <Overview
      sections={sections}
      experimentId={experiment?.id}
      headerItems={headerItems}
    />
  );
};
