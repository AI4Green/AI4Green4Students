import { useDisclosure } from "@chakra-ui/react";
import { FaTrash, FaLink } from "react-icons/fa";
import { GiMaterialsScience } from "react-icons/gi";
import { DeletePlanModal } from "../modal/DeletePlanModal";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";
import { PLAN_STAGES } from "constants/stages";

export const PlanOverviewAction = ({ plan, isInstructor }) => {
  const DeletePlanState = useDisclosure();
  const navigate = useNavigate();

  const {
    sectionTypes: { planSectionTypeId },
  } = plan.project;

  const planOverviewActions = {
    view: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "View",
      onClick: () =>
        navigate(`/project/${planSectionTypeId}/plan-overview/${plan.id}`),
    },
    ...(plan.status === PLAN_STAGES.Approved && // Show this option only after the plan is approved
      !isInstructor && {
        createReport: {
          isEligible: () => true,
          icon: <GiMaterialsScience />,
          label: "Start lab work/Report",
        },
      }),
    ...(!isInstructor && {
      delete: {
        isEligible: () => true,
        icon: <FaTrash />,
        label: "Delete",
        onClick: DeletePlanState.onOpen,
        colorScheme: "red",
      },
    }),
  };

  return (
    <>
      <ActionButton actions={planOverviewActions} size="xs" variant="outline" />
      {DeletePlanState.isOpen && (
        <DeletePlanModal
          isModalOpen={DeletePlanState.isOpen}
          onModalClose={DeletePlanState.onClose}
          plan={plan}
        />
      )}
    </>
  );
};

export const LiteratureReviewAction = ({ literatureReview, isInstructor }) => {
  const literatureReviewActions = {
    view: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "View",
    },
    ...(!isInstructor && {
      delete: {
        isEligible: () => true,
        icon: <FaTrash />,
        label: "Delete",
        colorScheme: "red",
      },
    }),
  };

  return (
    <ActionButton
      actions={literatureReviewActions}
      size="xs"
      variant="outline"
    />
  );
};
