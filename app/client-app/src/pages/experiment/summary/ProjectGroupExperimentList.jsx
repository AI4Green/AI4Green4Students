import { useParams } from "react-router-dom";
import { Summary } from ".";
import { useProjectSummaryByProjectGroup } from "api/projects";

export const ProjectGroupExperimentList = () => {
  const { projectId, projectGroupId } = useParams();
  const { data: projectSummary } =
    useProjectSummaryByProjectGroup(projectGroupId);

  return <Summary projectId={projectId} projectSummary={projectSummary} />;
};
