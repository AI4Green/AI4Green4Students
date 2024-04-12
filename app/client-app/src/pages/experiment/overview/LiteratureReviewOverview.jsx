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

  const lrSections = sections?.map((section) => ({
    ...section,
    path: `/project/${section.sectionType?.name}-section/${literatureReviewId}/${section.id}`,
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

  return <Overview sections={lrSections} headerItems={headerItems} />;
};
