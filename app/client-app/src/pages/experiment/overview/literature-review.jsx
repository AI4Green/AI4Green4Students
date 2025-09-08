import {
  useLiteratureReview,
  useLiteratureReviewSectionsList,
  useProjectGroup,
} from "api";
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

export const LiteratureReviewOverview = () => {
  const { user } = useUser();
  const { projectId, projectGroupId, literatureReviewId } = useParams();
  const { data: literatureReview, mutate } =
    useLiteratureReview(literatureReviewId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);

  const { data: sections } =
    useLiteratureReviewSectionsList(literatureReviewId);
  const lrSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.LiteratureReview,
      projectId,
      projectGroup?.id,
      literatureReviewId,
      section.id
    ),
  }));

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = literatureReview?.ownerId === user.userId;

  if (!literatureReview) return <NotFound />;

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
        isEverySectionApproved={sections?.every((section) => section.approved)}
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
    overviewTitle: "Literature Review Overview",
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

  return (
    <Overview
      stage={literatureReview?.stage}
      sections={lrSections}
      headerItems={headerItems}
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
      isOwner={isOwner}
      isInstructor={isInstructor}
    />
  );
};
