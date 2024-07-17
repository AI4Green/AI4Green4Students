import { Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { useReport, useReportSectionsList } from "api/report";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { Breadcrumbs } from "components/Breadcrumbs";
import { Box } from "@chakra-ui/react";

export const ReportOverview = () => {
  const { projectId, reportId } = useParams();
  const { data: report } = useReport(reportId);
  const { data: sections } = useReportSectionsList(reportId);

  const reportSections = sections?.map((section) => ({
    ...section,
    path: `/projects/${projectId}/reports/${reportId}/sections/${section.id}`,
  }));

  if (!report) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Report,
    header: `${report?.title || reportId}`,
    projectName: report?.projectName,
    owner: report?.ownerName,
    overviewTitle: "Report Overview",
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: report?.projectName,
      href: `/projects/${projectId}`,
    },
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
