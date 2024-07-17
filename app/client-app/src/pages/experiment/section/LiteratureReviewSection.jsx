import { useParams } from "react-router-dom";
import { Section } from ".";
import {
  useLiteratureReview,
  useLiteratureReviewSection,
} from "api/literatureReview";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { Breadcrumbs } from "components/Breadcrumbs";
import { Box } from "@chakra-ui/react";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";

export const LiteratureReviewSection = () => {
  const { projectId, literatureReviewId, sectionId } = useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);
  const { data: literatureReviewSection, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  const { literatureReviews } = useBackendApi();

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
      href: `/projects/${projectId}`,
    },
    {
      label: "Literature Review",
      href: `/projects/${projectId}/literature-reviews/${literatureReviewId}/overview`,
    },
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
