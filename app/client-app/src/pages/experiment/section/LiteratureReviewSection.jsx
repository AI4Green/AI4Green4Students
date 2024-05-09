import { useParams } from "react-router-dom";
import { Section } from ".";
import {
  useLiteratureReview,
  useLiteratureReviewSection,
} from "api/literatureReview";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";

export const LiteratureReviewSection = () => {
  const { literatureReviewId, sectionId } = useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);
  const { data: literatureReviewSection, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  const { literatureReviews } = useBackendApi();

  const headerItems = {
    header: `Literature Review - ${
      literatureReview?.title ?? literatureReviewId
    }`,
    subHeader: literatureReview?.projectName,
    overviewTitle: literatureReviewSection?.name,
  };

  return (
    <Section
      record={literatureReview}
      section={literatureReviewSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.LiteratureReview}
      headerItems={headerItems}
      save={literatureReviews.saveFieldResponses}
    />
  );
};
