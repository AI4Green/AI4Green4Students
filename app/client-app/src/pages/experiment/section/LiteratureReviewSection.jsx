import { useParams } from "react-router-dom";
import { Section } from ".";
import {
  useLiteratureReview,
  useLiteratureReviewSection,
} from "api/literatureReview";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { useLiteratureReviewSectionsList } from "api/literatureReview";
import { buildOverviewPath, buildProjectPath } from "routes/Project";

export const LiteratureReviewSection = () => {
  const { projectId, literatureReviewId, sectionId } = useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);
  const { data: literatureReviewSection, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  const { data: sections } =
    useLiteratureReviewSectionsList(literatureReviewId);
  const { literatureReviews } = useBackendApi();
  const isInstructor = useIsInstructor();

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.LiteratureReview,
    header: `${literatureReview?.title || literatureReviewId}`,
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
    ...(isInstructor
      ? [
          {
            label: literatureReview?.ownerName,
            href: buildProjectPath(projectId, true, literatureReview?.ownerId),
          },
        ]
      : []),
    ...(sections?.count > 1 // Only show overview link if there are multiple sections
      ? [
          {
            label: "Literature Review",
            href: buildOverviewPath(
              SECTION_TYPES.LiteratureReview,
              projectId,
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
