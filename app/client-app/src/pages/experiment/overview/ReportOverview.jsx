import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { useReport, useReportSectionsList } from "api/report";

export const ReportOverview = () => {
  const { projectId, projectGroupId, reportId } = useParams();
  const { data: report, mutate } = useReport(reportId);

  const { data: sections } = useReportSectionsList(reportId);

  const reportSections = sections?.map((section) => ({
    ...section,
    path: `/projects/${projectId}/reports/${reportId}/sections/${section.id}`,
  }));

  if (!report) return <NotFound />;

  const headerItems = {
    header: `Report - ${report?.title ?? reportId}`,
    subHeader: report?.projectName,
    owner: report?.ownerName,
    overviewTitle: "Report Overview",
  };

  return (
    <Overview
      sections={reportSections}
      headerItems={headerItems}
      InstructorAction={
        <InstructorAction
          record={{ ...report, projectId, projectGroupId, mutate }}
          isEverySectionApproved={sections?.every(
            (section) => section.approved
          )}
          isReport
        />
      }
    />
  );
};
