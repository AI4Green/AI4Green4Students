import { usePlan, usePlanSection, useProjectGroup } from "api";
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

export const PlanSection = () => {
  const { planId, projectId, projectGroupId, sectionId } = useParams();

  const { user } = useUser();
  const { data: plan } = usePlan(planId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = plan?.owner.id === user.userId;

  const { data: form, mutate } = usePlanSection(planId, sectionId);
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );
  const { plans } = useBackendApi();

  if (!plan) return <NotFound />;

  const headerItems = {
    header: {
      type: "Plan",
      icon: TITLE_ICON_COMPONENTS.Plan,
      title: plan?.title,
    },
    project: plan?.project,
    owner: plan?.owner,
  };

  const breadcrumbItems = [
    { label: "Projects", href: "/projects" },
    {
      label: plan?.project.name,
      href: buildProjectPath(projectId),
    },
    ...(!isOwner
      ? [
          {
            label: projectGroup?.name,
            href:
              !isInstructor &&
              buildStudentsProjectGroupPath(projectId, projectGroup?.id),
          },
          {
            label: plan?.owner.name,
            href: buildProjectPath(projectId, projectGroup?.id, plan?.owner.id),
          },
        ]
      : []),
    {
      label: plan?.title,
      href: buildOverviewPath(
        SECTION_TYPES.Plan,
        projectId,
        projectGroupId,
        plan?.id
      ),
    },
    {
      label: form?.name,
    },
  ];

  const item = {
    id: plan.id,
    type: SECTION_TYPES.Plan,
    stage: {
      name: plan?.stage,
      permissions: plan?.permissions,
    },
    isOwner,
    action: {
      mutate,
      save: plans.saveFieldResponses,
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
