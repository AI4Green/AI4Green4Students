import { useParams } from "react-router-dom";
import { ExperimentList } from "components/experiment/ExperimentList";
import { useProjectSummaryByProjectGroup } from "api/projects";

export const ProjectGroupExperimentList = () => {
  const { projectId, projectGroupId } = useParams();
  const { data: projectSummary } =
    useProjectSummaryByProjectGroup(projectGroupId);

  return (
    <ExperimentList projectId={projectId} projectSummary={projectSummary} />
  );
};
