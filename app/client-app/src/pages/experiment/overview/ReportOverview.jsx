import { Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { useReport, useReportSectionsList } from "api/report";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { Breadcrumbs } from "components/Breadcrumbs";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { SECTION_TYPES } from "constants/section-types";
import { buildSectionFormPath, buildProjectPath } from "routes/Project";

export const ReportOverview = () => {
  const { projectId, reportId } = useParams();
  const { data: report } = useReport(reportId);
  const { data: sections } = useReportSectionsList(reportId);

  const reportSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.Report,
      projectId,
      reportId,
      section.id
    ),
  }));

  const isInstructor = useIsInstructor();

  if (!report) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Report,
    header: `${report?.title || reportId}`,
    projectName: report?.projectName,
    owner: report?.ownerName,
    ownerId: report?.ownerId,
    overviewTitle: "Report Overview",
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
      label: report?.title,
    },
  ];

  return (
    <Overview
      sections={reportSections}
      headerItems={headerItems}
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
