import {
  useLiteratureReview,
  useLiteratureReviewSectionsList,
  useProjectGroup,
} from "api";
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

export const LiteratureReviewOverview = () => {
  const { projectId, projectGroupId, literatureReviewId } = useParams();

  const { user } = useUser();
  const { data: literatureReview, mutate } =
    useLiteratureReview(literatureReviewId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = literatureReview?.owner.id === user.userId;

  const { data: sections } =
    useLiteratureReviewSectionsList(literatureReviewId);
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );

  if (!literatureReview) return <NotFound />;

  const lrSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.LiteratureReview,
      projectId,
      projectGroupId,
      literatureReviewId,
      section.id
    ),
  }));

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.LiteratureReview,
    header: {
      type: "Literature Review",
      icon: TITLE_ICON_COMPONENTS.LiteratureReview,
      title: literatureReview?.title,
    },
    project: literatureReview?.project,
    owner: literatureReview?.owner,
    action: isInstructor ? (
      <InstructorActions
        record={{ ...literatureReview, mutate }}
        isEverySectionApproved={sections?.every(
          (section) => section.feedback.approved
        )}
        sectionType={SECTION_TYPES.LiteratureReview}
        sections={lrSections}
      />
    ) : (
      <StudentActions
        record={{ ...literatureReview, mutate }}
        sectionType={SECTION_TYPES.LiteratureReview}
        sections={lrSections}
      />
    ),
  };

  const breadcrumbItems = [
    { label: "Projects", href: "/projects" },
    {
      label: literatureReview?.project.name,
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
            label: literatureReview?.owner.name,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              literatureReview?.owner.id
            ),
          },
        ]
      : []),
    {
      label: "Literature Review",
    },
  ];

  const item = {
    type: SECTION_TYPES.LiteratureReview,
    stage: {
      name: literatureReview?.stage,
      permissions: literatureReview?.permissions,
    },
    isOwner,
  };

  return (
    <Overview
      item={item}
      sections={lrSections}
      headerItems={headerItems}
      breadcrumbs={breadcrumbItems}
      isInstructor={isInstructor}
    />
  );
};
