import { useProjectGroup, useReport, useReportSectionsList } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
import {
  InstructorActions,
  StudentActions,
} from "components/experiment-summary";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useUser } from "contexts";
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

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = report?.ownerId === user.userId;

  if (!report) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Report,
    header: {
      type: "Report",
      icon: TITLE_ICON_COMPONENTS.Report,
      title: report?.title,
    },
    project: report?.project,
    owner: report?.owner,
    action: isInstructor ? (
      <InstructorActions
        record={{ ...report, mutate }}
        sectionType={SECTION_TYPES.Report}
        sections={reportSections}
      />
    ) : (
      <StudentActions
        record={{ ...report }}
        sectionType={SECTION_TYPES.Report}
        sections={reportSections}
      />
    ),
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: report?.project.name,
      href: buildProjectPath(projectId),
    },
    ...(!isOwner
      ? [
          {
            label: projectGroup.name,
            href:
              !isInstructor &&
              buildStudentsProjectGroupPath(projectId, projectGroup?.id),
          },
          {
            label: report?.owner.name,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              report?.owner.id
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
      stage={report?.stage}
      sections={reportSections}
      headerItems={headerItems}
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
      isOwner={isOwner}
      isInstructor={isInstructor}
    />
  );
};
