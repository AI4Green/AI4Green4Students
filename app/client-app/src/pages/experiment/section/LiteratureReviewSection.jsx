import { useParams } from "react-router-dom";
import { Section } from ".";
import { useLiteratureReview } from "api/literatureReview";
import { useLiteratureReviewSection } from "api/section";
import { SECTION_TYPES } from "constants/section-types";

export const LiteratureReviewSection = () => {
  const { literatureReviewId, sectionId } = useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);
  const { data: literatureReviewSection, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  return (
    <Section
      record={literatureReview}
      section={literatureReviewSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.LiteratureReview}
    />
  );
};
