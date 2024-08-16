import { useParams } from "react-router-dom";
import { Section } from ".";
import { useReport, useReportSection } from "api/report";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { buildOverviewPath, buildProjectPath } from "routes/Project";

export const ReportSection = () => {
  const { reportId, sectionId, projectId } = useParams();
  const { data: report } = useReport(reportId);
  const { data: reportSection, mutate } = useReportSection(reportId, sectionId);
  const { reports } = useBackendApi();

  const isInstructor = useIsInstructor();

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Report,
    header: `${report?.title || reportId}`,
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
    ...(isInstructor
      ? [
          {
            label: report?.ownerName,
            href: buildProjectPath(projectId, isInstructor, report?.ownerId),
          },
        ]
      : []),

    {
      label: "Report",
      href: buildOverviewPath(SECTION_TYPES.Report, projectId, reportId),
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
