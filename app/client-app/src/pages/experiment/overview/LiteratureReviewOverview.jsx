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
import { buildSectionFormPath, buildProjectPath } from "routes/Project";

export const LiteratureReviewOverview = () => {
  const { projectId, literatureReviewId } = useParams();
  const { data: literatureReview, mutate } =
    useLiteratureReview(literatureReviewId);

  const { data: sections } =
    useLiteratureReviewSectionsList(literatureReviewId);

  const lrSections = sections?.map((section) => ({
    ...section,
    path: buildSectionFormPath(
      SECTION_TYPES.LiteratureReview,
      projectId,
      literatureReviewId,
      section.id
    ),
  }));

  const isInstructor = useIsInstructor();

  if (!literatureReview) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.LiteratureReview,
    header: `${literatureReview?.title || literatureReviewId}`,
    projectName: literatureReview?.projectName,
    owner: literatureReview?.ownerName,
    overviewTitle: "Literature Review Overview",
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
            href: buildProjectPath(
              projectId,
              isInstructor,
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
