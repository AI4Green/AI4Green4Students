import { useProjectGroup, useReport, useReportSection } from "api";
import { SectionForm } from "components/section-form";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi, useUser } from "contexts";
import { NotFound } from "pages/error";
import { useParams } from "react-router-dom";
import {
  buildOverviewPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

export const ReportSection = () => {
  const { reportId, sectionId, projectId, projectGroupId } = useParams();

  const { user } = useUser();
  const { data: report } = useReport(reportId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = report?.owner.id === user.userId;

  const { data: form, mutate } = useReportSection(reportId, sectionId);
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );
  const { reports } = useBackendApi();

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
      label: report?.title || "Report",
      href: buildOverviewPath(
        SECTION_TYPES.Report,
        projectId,
        projectGroupId,
        reportId
      ),
    },
    {
      label: form?.name,
    },
  ];

  const item = {
    id: report.id,
    type: SECTION_TYPES.Report,
    stage: {
      name: report?.stage,
      permissions: report?.permissions,
    },
    isOwner,
    action: {
      mutate,
      save: reports.saveFieldResponses,
    },
  };

  return (
    <SectionForm
      item={item}
      form={form}
      headerItems={headerItems}
      breadcrumbItems={breadcrumbItems}
      isInstructor={isInstructor}
    />
  );
};
