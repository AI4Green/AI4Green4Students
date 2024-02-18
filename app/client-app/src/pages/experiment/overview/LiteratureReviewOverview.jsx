import { Overview } from ".";
import { useParams } from "react-router-dom";
import { useLiteratureReviewSectionsList } from "api/section";
import { NotFound } from "pages/error/NotFound";
import { useLiteratureReview } from "api/literatureReview";

export const LiteratureReviewOverview = () => {
  const { literatureReviewId, sectionTypeId } = useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);

  const { data: sections } = useLiteratureReviewSectionsList(
    literatureReviewId,
    sectionTypeId
  );

  if (!literatureReview) return <NotFound />;

  const headerItems = {
    header:
      literatureReview?.title ?? `Literature Review ${literatureReviewId}`,
    subHeader: literatureReview?.projectName,
    owner: literatureReview?.ownerName,
    overviewTitle: "Literature Review Overview",
  };

  return (
    <Overview
      sections={sections}
      recordId={literatureReview?.id}
      headerItems={headerItems}
    />
  );
};
