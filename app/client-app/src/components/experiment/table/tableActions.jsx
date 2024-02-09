import { useDisclosure } from "@chakra-ui/react";
import { FaTrash, FaLink } from "react-icons/fa";
import { GiMaterialsScience } from "react-icons/gi";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";
import { PLAN_STAGES } from "constants/stages";
import { DeleteModal } from "../modal/DeleteModal";

export const PlanOverviewAction = ({ plan, isInstructor }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
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
        onClick: onOpen,
        colorScheme: "red",
      },
    }),
  };

  return (
    <>
      <ActionButton actions={planOverviewActions} size="xs" variant="outline" />
      {isOpen && (
        <DeleteModal
          isModalOpen={isOpen}
          onModalClose={onClose}
          record={plan}
          isPlan
        />
      )}
    </>
  );
};

export const LiteratureReviewAction = ({ literatureReview, isInstructor }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const navigate = useNavigate();
  const {
    sectionTypes: { literatureReviewSectionTypeId },
  } = literatureReview.project;

  const literatureReviewActions = {
    view: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "View",
      onClick: () =>
        navigate(
          `/project/${literatureReviewSectionTypeId}/literatureReview-overview/${literatureReview.id}`
        ),
    },
    ...(!isInstructor && {
      delete: {
        isEligible: () => true,
        icon: <FaTrash />,
        label: "Delete",
        onClick: onOpen,
        colorScheme: "red",
      },
    }),
  };

  return (
    <>
      <ActionButton
        actions={literatureReviewActions}
        size="xs"
        variant="outline"
      />
      {isOpen && (
        <DeleteModal
          isModalOpen={isOpen}
          onModalClose={onClose}
          record={literatureReview}
          isLiteratureReview
        />
      )}
    </>
  );
};
