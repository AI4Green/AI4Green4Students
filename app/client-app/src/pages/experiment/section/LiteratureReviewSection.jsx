import { useParams } from "react-router-dom";
import { Section } from "./Section";
import {
  useLiteratureReview,
  useLiteratureReviewSection,
  useLiteratureReviewSectionsList,
  useProjectGroup,
} from "api";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi, useUser } from "contexts";
import { useIsInstructor } from "helpers/hooks";
import {
  buildOverviewPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";

export const LiteratureReviewSection = () => {
  const { user } = useUser();
  const { projectId, projectGroupId, literatureReviewId, sectionId } =
    useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);
  const { data: literatureReviewSection, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  const { data: sections } =
    useLiteratureReviewSectionsList(literatureReviewId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { literatureReviews } = useBackendApi();

  const isInstructor = useIsInstructor();
  const isAuthor = literatureReview?.ownerId === user.userId;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.LiteratureReview,
    header: literatureReview?.title,
    projectName: literatureReview?.projectName,
    owner: literatureReview?.ownerName,
    overviewTitle: `${literatureReviewSection?.name} Form`,
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
    ...(sections?.count > 1 // Only show overview link if there are multiple sections
      ? [
          {
            label: literatureReview?.title || "Literature Review",
            href: buildOverviewPath(
              SECTION_TYPES.LiteratureReview,
              projectId,
              projectGroup?.id,
              literatureReviewId
            ),
          },
        ]
      : []),

    {
      label: literatureReviewSection?.name,
    },
  ];

  return (
    <Section
      record={literatureReview}
      section={literatureReviewSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.LiteratureReview}
      headerItems={headerItems}
      breadcrumbItems={breadcrumbItems}
      save={literatureReviews.saveFieldResponses}
    />
  );
};
