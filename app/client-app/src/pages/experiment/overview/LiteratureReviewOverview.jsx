import { InstructorAction, Overview } from ".";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import {
  useLiteratureReview,
  useLiteratureReviewSectionsList,
} from "api/literatureReview";
import { Breadcrumbs } from "components/Breadcrumbs";
import { Box } from "@chakra-ui/react";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";

export const LiteratureReviewOverview = () => {
  const { projectId, literatureReviewId } = useParams();
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
      href: `/projects/${projectId}`,
    },
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
          isLiteratureReview
        />
      }
    />
  );
};
