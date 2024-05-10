import { useParams } from "react-router-dom";
import { Section } from ".";
import { useReport, useReportSection } from "api/report";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";

export const ReportSection = () => {
  const { reportId, sectionId } = useParams();
  const { data: report } = useReport(reportId);
  const { data: reportSection, mutate } = useReportSection(reportId, sectionId);
  const { reports } = useBackendApi();

  const headerItems = {
    header: `Report - ${report?.title ?? reportId}`,
    subHeader: report?.projectName,
    overviewTitle: reportSection?.name,
  };

  return (
    <Section
      record={report}
      section={reportSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Report}
      headerItems={headerItems}
      save={reports.saveFieldResponses}
    />
  );
};
