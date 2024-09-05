import { useParams } from "react-router-dom";
import { Section } from ".";
import { useReport, useReportSection } from "api/report";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import {
  buildOverviewPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts/User";
import { useProjectGroup } from "api/projectGroups";

export const ReportSection = () => {
  const { user } = useUser();
  const { reportId, sectionId, projectId, projectGroupId } = useParams();
  const { data: report } = useReport(reportId);
  const { data: reportSection, mutate } = useReportSection(reportId, sectionId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { reports } = useBackendApi();

  const isInstructor = useIsInstructor();
  const isAuthor = report?.ownerId === user.userId;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Report,
    header: report?.title,
    projectName: report?.projectName,
    owner: report?.ownerName,
    overviewTitle: `${reportSection?.name} Form`,
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: report?.projectName,
      href: buildProjectPath(projectId),
    },
    ...(!isAuthor
      ? [
          {
            label: projectGroup.name,
            href:
              !isInstructor &&
              buildStudentsProjectGroupPath(projectId, projectGroup?.id),
          },
          {
            label: report?.ownerName,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              report?.ownerId
            ),
          },
        ]
      : []),
    {
      label: report?.title || "Report",
      href: buildOverviewPath(
        SECTION_TYPES.Report,
        projectId,
        projectGroup?.id,
        reportId
      ),
    },
    {
      label: reportSection?.name,
    },
  ];

  return (
    <Section
      record={report}
      section={reportSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Report}
      headerItems={headerItems}
      save={reports.saveFieldResponses}
      breadcrumbItems={breadcrumbItems}
    />
  );
};
