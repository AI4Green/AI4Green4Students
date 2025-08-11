import { useProjectGroup, useReport, useReportSectionsList } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
import {
  InstructorActions,
  StudentActions,
} from "components/experiment-summary";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { useUser } from "contexts";
import { useIsInstructor } from "helpers/hooks";
import { NotFound } from "pages/error";
import { useParams } from "react-router-dom";
import {
  buildProjectPath,
  buildSectionFormPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

import { Overview } from "./overview";

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
        <InstructorActions
          record={{ ...report, mutate }}
          sections={reportSections}
          sectionType={SECTION_TYPES.Report}
        />
      }
      StudentAction={
        <StudentActions
          record={{ ...report, mutate }}
          sectionType={SECTION_TYPES.Report}
          sections={reportSections}
        />
      }
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
