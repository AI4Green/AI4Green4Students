import { Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { useReport, useReportSectionsList } from "api/report";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { Breadcrumbs } from "components/core/Breadcrumbs";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { SECTION_TYPES } from "constants/section-types";
import {
  buildSectionFormPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts/User";
import { useProjectGroup } from "api/projectGroups";
import { InstructorAction } from "components/experiment-summary";

export const ReportOverview = () => {
  const { user } = useUser();
  const { projectId, projectGroupId, reportId } = useParams();
  const { data: report, mutate } = useReport(reportId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);

  const { data: sections } = useReportSectionsList(reportId);
  const reportSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.Report,
      projectId,
      projectGroup?.id,
      reportId,
      section.id
    ),
  }));

  const isInstructor = useIsInstructor();
  const isAuthor = report?.ownerId === user.userId;

  if (!report) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Report,
    header: report?.title,
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
      label: report?.title,
    },
  ];

  return (
    <Overview
      sections={reportSections}
      headerItems={headerItems}
      InstructorAction={
        <InstructorAction
          record={{ ...report, mutate }}
          isEverySectionApproved={sections?.every(
            (section) => section.approved
          )}
          sectionType={SECTION_TYPES.Plan}
        />
      }
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
