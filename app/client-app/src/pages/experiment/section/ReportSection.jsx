import { useParams } from "react-router-dom";
import { Section } from ".";
import { useReport, useReportSection } from "api/report";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { Box } from "@chakra-ui/react";

export const ReportSection = () => {
  const { reportId, sectionId, projectId } = useParams();
  const { data: report } = useReport(reportId);
  const { data: reportSection, mutate } = useReportSection(reportId, sectionId);
  const { reports } = useBackendApi();

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
      href: `/projects/${projectId}`,
    },
    {
      label: "Report",
      href: `/projects/${projectId}/reports/${reportId}/overview`,
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
