import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import {
  useLiteratureReview,
  useLiteratureReviewSectionsList,
} from "api/literatureReview";

export const LiteratureReviewOverview = () => {
  const { projectId, projectGroupId, literatureReviewId } = useParams();
  const { data: literatureReview, mutate } =
    useLiteratureReview(literatureReviewId);

  const { data: sections } =
    useLiteratureReviewSectionsList(literatureReviewId);

  const lrSections = sections?.map((section) => ({
    ...section,
    path: `/projects/${projectId}/literature-reviews/${literatureReviewId}/sections/${section.id}`,
  }));

  if (!literatureReview) return <NotFound />;

  const headerItems = {
    header: `Literature Review - ${
      literatureReview?.title ?? literatureReviewId
    }`,
    subHeader: literatureReview?.projectName,
    owner: literatureReview?.ownerName,
    overviewTitle: "Literature Review Overview",
  };

  return (
    <Overview
      sections={lrSections}
      headerItems={headerItems}
      InstructorAction={
        <InstructorAction
          record={{ ...literatureReview, projectId, projectGroupId, mutate }}
          isEverySectionApproved={sections?.every(
            (section) => section.approved
          )}
          isLiteratureReview
        />
      }
    />
  );
};
