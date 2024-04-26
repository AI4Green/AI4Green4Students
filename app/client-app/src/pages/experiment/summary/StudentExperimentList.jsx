import { useParams } from "react-router-dom";
import { Summary } from ".";
import { useProjectSummaryByStudent } from "api/projects";

export const StudentExperimentList = () => {
  const { projectId } = useParams();
  const { data: projectSummary } = useProjectSummaryByStudent(projectId);

  return <Summary projectId={projectId} projectSummary={projectSummary} />;
};
