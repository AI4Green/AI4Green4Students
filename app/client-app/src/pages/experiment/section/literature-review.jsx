import {
  useLiteratureReview,
  useLiteratureReviewSection,
  useProjectGroup,
  useSectionsListBySectionType,
} from "api";
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

export const LiteratureReviewSection = () => {
  const { projectId, projectGroupId, literatureReviewId, sectionId } =
    useParams();

  const { user } = useUser();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = literatureReview?.owner.id === user.userId;

  const { data: form, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );
  const { data: sections } = useSectionsListBySectionType(
    projectId,
    SECTION_TYPES.LiteratureReview
  );
  const { literatureReviews } = useBackendApi();

  if (!literatureReview) return <NotFound />;

  const headerItems = {
    header: {
      type: "Literature Review",
      icon: TITLE_ICON_COMPONENTS.LiteratureReview,
      title: literatureReview?.title,
    },
    project: literatureReview?.project,
    owner: literatureReview?.owner,
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: literatureReview?.project.name,
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
            label: literatureReview?.owner.name,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              literatureReview?.owner.id
            ),
          },
        ]
      : []),
    ...(sections?.count > 1 // Only show overview link if there are multiple sections
      ? [
          {
            label: literatureReview?.title || "Literature Review",
            href: buildOverviewPath(
              SECTION_TYPES.LiteratureReview,
              projectId,
              projectGroupId,
              literatureReviewId
            ),
          },
        ]
      : []),

    {
      label: form?.name,
    },
  ];

  const item = {
    id: literatureReview.id,
    type: SECTION_TYPES.LiteratureReview,
    stage: {
      name: literatureReview?.stage,
      permissions: literatureReview?.permissions,
    },
    isOwner,
    action: {
      mutate,
      save: literatureReviews.saveFieldResponses,
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
