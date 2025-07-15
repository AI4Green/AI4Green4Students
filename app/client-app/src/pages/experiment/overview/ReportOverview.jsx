import { Overview } from "./Overview";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error";
import { useReport, useReportSectionsList, useProjectGroup } from "api";
import { TITLE_ICON_COMPONENTS, SECTION_TYPES } from "constants";
import { Breadcrumbs } from "components/core/Breadcrumbs";
import { useIsInstructor } from "helpers/hooks";
import {
  buildSectionFormPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts";
import { InstructorAction, StudentAction } from "components/experiment-summary";

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
          sections={reportSections}
          sectionType={SECTION_TYPES.Report}
        />
      }
      StudentAction={
        <StudentAction
          record={{ ...report, mutate }}
          sectionType={SECTION_TYPES.Report}
          sections={reportSections}
        />
      }
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
