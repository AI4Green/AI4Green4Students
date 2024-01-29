import { useParams } from "react-router-dom";
import { ExperimentList } from "components/experiment/ExperimentList";
import { useProjectSummaryByStudent } from "api/projects";

export const StudentExperimentList = () => {
  const { projectId } = useParams();
  const { data: projectSummary } = useProjectSummaryByStudent(projectId);

  return (
    <ExperimentList projectId={projectId} projectSummary={projectSummary} />
  );
};
