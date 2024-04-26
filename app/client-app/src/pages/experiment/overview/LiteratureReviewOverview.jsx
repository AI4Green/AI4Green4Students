import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { useLiteratureReviewSectionsList } from "api/section";
import { NotFound } from "pages/error/NotFound";
import { useLiteratureReview } from "api/literatureReview";

export const LiteratureReviewOverview = () => {
  const { projectId, projectGroupId, literatureReviewId, sectionTypeId } =
    useParams();
  const { data: literatureReview, mutate } =
    useLiteratureReview(literatureReviewId);

  const { data: sections } = useLiteratureReviewSectionsList(
    literatureReviewId,
    sectionTypeId
  );

  const lrSections = sections?.map((section) => ({
    ...section,
    path: `/project/${projectId}/project-group/${projectGroupId}/literature-review/${literatureReviewId}/section/${section.id}`,
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
