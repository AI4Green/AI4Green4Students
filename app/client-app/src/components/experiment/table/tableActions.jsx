import { useDisclosure } from "@chakra-ui/react";
import { FaTrash, FaLink } from "react-icons/fa";
import { GiMaterialsScience } from "react-icons/gi";
import { DeletePlanModal } from "../modal/DeletePlanModal";
import { CreateOrEditPlanModal } from "../modal/CreateOrEditPlanModal";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";

export const PlanOverviewAction = ({ plan, isInstructor }) => {
  const DeletePlanState = useDisclosure();
  const EditPlanState = useDisclosure();
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
    ...(!isInstructor && {
      edit: {
        isEligible: () => true,
        icon: <GiMaterialsScience />,
        label: "Edit",
        onClick: EditPlanState.onOpen,
        colorScheme: "red",
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
      {EditPlanState.isOpen && (
        <CreateOrEditPlanModal
          isModalOpen={EditPlanState.isOpen}
          onModalClose={EditPlanState.onClose}
          existingPlanId={plan?.id}
          project={plan?.project}
        />
      )}
    </>
  );
};
