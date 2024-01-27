import { useParams } from "react-router-dom";
import { ExperimentList } from "components/experiment/ExperimentList";
import { usePlansList } from "api/plans";

export const StudentExperimentList = () => {
  const { projectId } = useParams();
  const { data: plans } = usePlansList(projectId);

  return <ExperimentList projectId={projectId} plans={plans} />;
};
