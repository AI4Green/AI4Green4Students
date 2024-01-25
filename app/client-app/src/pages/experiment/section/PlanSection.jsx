import { useToast } from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { usePlan } from "api/plans";
import { usePlanSection } from "api/section";
import { useUser } from "contexts/User";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
import { Section } from ".";
import { evaluateFieldCondition } from ".";

export const PlanSection = () => {
  const { user } = useUser();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { planId, sectionId } = useParams();
  const { data: plan } = usePlan(planId);

  const { data: planSection } = usePlanSection(planId, sectionId);
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
      let submissionData = {};
      try {
        setIsLoading(true);
        fields.forEach((field) =>
          evaluateFieldCondition(field, fields, values, submissionData)
        );
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

  const isInstuctor = user.permissions?.includes(
    EXPERIMENTS_PERMISSIONS.ViewAllExperiments
  );
  return (
    <Section
      isInstructor={isInstuctor}
      record={plan}
      isLoading={isLoading}
      section={planSection}
      handleSubmit={handleSubmit}
    />
  );
};
