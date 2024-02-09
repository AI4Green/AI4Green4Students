import { useToast } from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { Section } from ".";
import { useLiteratureReview } from "api/literatureReview";
import { useLiteratureReviewSection } from "api/section";
import { prepareSubmissionData } from "components/experiment/section/form/fieldEvaluation";

export const LiteratureReviewSection = () => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { literatureReviewId, sectionId } = useParams();
  const { data: literatureReview } = useLiteratureReview(literatureReviewId);

  const { data: literatureReviewSection, mutate } = useLiteratureReviewSection(
    literatureReviewId,
    sectionId
  );
  const { t } = useTranslation();
  const toast = useToast();

  useEffect(() => {
    feedback &&
      toast({
        position: "top",
        title: feedback.message,
        status: feedback.status,
        duration: 1500,
        isClosable: true,
      });
  }, [feedback]);

  const handleSubmit = async (values, fields) => {
    /*
      TODO: Send the field responses to the backend and process them accordingly
    try {
      setIsLoading(true);
      const submissionData = prepareSubmissionData(fields, values);
      console.log({ ...submissionData, sectionId, planId });

      setFeedback({
        status: "success",
        message: "Section response values saved",
      });
      setIsLoading(false);
    } catch (e) {
      console.error(e);
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
    */
  };

  return (
    <Section
      record={literatureReview}
      isLoading={isLoading}
      section={literatureReviewSection}
      mutate={mutate}
      handleSubmit={handleSubmit}
    />
  );
};
