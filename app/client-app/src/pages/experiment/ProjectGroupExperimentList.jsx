import { useParams } from "react-router-dom";
import { usePlansListByProjectGroup } from "api/plans";
import { ExperimentList } from "components/experiment/ExperimentList";

export const ProjectGroupExperimentList = () => {
  const { projectId, projectGroupId } = useParams();
  const { data: plans } = usePlansListByProjectGroup(projectGroupId);

  return <ExperimentList projectId={projectId} plans={plans} />;
};
