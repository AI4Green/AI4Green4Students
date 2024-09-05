import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import {
  useLiteratureReview,
  useLiteratureReviewSectionsList,
} from "api/literatureReview";
import { Breadcrumbs } from "components/Breadcrumbs";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { SECTION_TYPES } from "constants/section-types";
import {
  buildSectionFormPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts/User";
import { useProjectGroup } from "api/projectGroups";

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

  const isInstructor = useIsInstructor();
  const isAuthor = literatureReview?.ownerId === user.userId;

  if (!literatureReview) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.LiteratureReview,
    header: literatureReview?.title,
    projectName: literatureReview?.projectName,
    owner: literatureReview?.ownerName,
    ownerId: literatureReview?.ownerId,
    overviewTitle: "Literature Review Overview",
  };
  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: literatureReview?.projectName,
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
            label: literatureReview?.ownerName,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              literatureReview?.ownerId
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
      sections={lrSections}
      headerItems={headerItems}
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
      InstructorAction={
        <InstructorAction
          record={{ ...literatureReview, mutate }}
          isEverySectionApproved={sections?.every(
            (section) => section.approved
          )}
          sectionType={SECTION_TYPES.LiteratureReview}
        />
      }
    />
  );
};
