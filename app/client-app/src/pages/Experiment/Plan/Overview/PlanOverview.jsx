import { Overview } from ".";
import { useParams } from "react-router-dom";
import { useSectionsList } from "api/section";
import { useExperiment } from "api/experiments";

export const PlanOverview = () => {
  const { experimentId } = useParams();
  const { data: experiment } = useExperiment(experimentId);

  // TODO: currently, all sections are returned. only get non-reaction sections.
  const { data: sections } = useSectionsList(experimentId);

  return (
    <Overview
      sections={sections}
      overview="Experiment Plan Overview"
      header={experiment?.title}
      subHeader={experiment?.projectName}
    />
  );
};
