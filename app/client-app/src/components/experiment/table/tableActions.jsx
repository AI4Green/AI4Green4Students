import { useDisclosure } from "@chakra-ui/react";
import { FaTrash, FaLink, FaExpandArrowsAlt } from "react-icons/fa";
import { GiMaterialsScience } from "react-icons/gi";
import { DeleteExperimentModal } from "../modal/DeleteExperimentModal";
import { DeleteExperimentReactionModal } from "../modal/DeleteExperimentReactionModal";
import { CreateOrEditExperimentModal } from "../modal/CreateOrEditExperimentModal";
import { CreateOrEditExperimentReactionModal } from "../modal/CreateOrEditExperimentReactionModal";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";

export const ExperimentAction = ({ row }) => {
  const DeleteExperimentState = useDisclosure();
  const EditExperimentState = useDisclosure();
  const NewReactionState = useDisclosure();

  const experimentActions = {
    newReaction: {
      isEligible: () => true,
      icon: <GiMaterialsScience />,
      label: "New Reaction",
      onClick: NewReactionState.onOpen,
    },
    expandCollapse: {
      isEligible: () => true,
      icon: <FaExpandArrowsAlt />,
      label: row.getIsExpanded() ? "Collapse" : "Expand",
      onClick: () => row.toggleExpanded(),
    },
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: EditExperimentState.onOpen,
    },
    delete: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Delete",
      onClick: DeleteExperimentState.onOpen,
      colorScheme: "red",
    },
  };

  return (
    <>
      <ActionButton actions={experimentActions} size="xs" variant="outline" />
      {DeleteExperimentState.isOpen && (
        <DeleteExperimentModal
          isModalOpen={DeleteExperimentState.isOpen}
          onModalClose={DeleteExperimentState.onClose}
          experiment={row.original}
        />
      )}
      {EditExperimentState.isOpen && (
        <CreateOrEditExperimentModal
          isModalOpen={EditExperimentState.isOpen}
          onModalClose={EditExperimentState.onClose}
          experiment={row.original}
          projectGroup={row.original.project.projectGroups[0]}
        />
      )}
      {NewReactionState.isOpen && (
        <CreateOrEditExperimentReactionModal
          isModalOpen={NewReactionState.isOpen}
          onModalClose={NewReactionState.onClose}
          experiment={row.original}
        />
      )}
    </>
  );
};

export const OverviewAction = ({ experiment }) => {
  const navigate = useNavigate();
  return (
    <>
      <ActionButton
        actions={{
          view: {
            isEligible: () => true,
            icon: <FaLink />,
            label: "View",
            onClick: () =>
              navigate(`/experiments/${experiment.experimentId}/plan-overview`),
          },
        }}
        size="xs"
        variant="outline"
      />
    </>
  );
};

export const ReactionOverviewAction = ({ reaction, isInstructor }) => {
  const DeleteReactionState = useDisclosure();
  const EditReactionState = useDisclosure();
  const navigate = useNavigate();

  const reactionOverviewActions = {
    view: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "View",
      onClick: () =>
        navigate(
          `/experiments/${reaction.experimentId}/reaction-overview/${reaction.id}`
        ),
    },
    ...(!isInstructor && {
      edit: {
        isEligible: () => true,
        icon: <GiMaterialsScience />,
        label: "Edit",
        onClick: EditReactionState.onOpen,
        colorScheme: "red",
      },
    }),
    ...(!isInstructor && {
      delete: {
        isEligible: () => true,
        icon: <FaTrash />,
        label: "Delete",
        onClick: DeleteReactionState.onOpen,
        colorScheme: "red",
      },
    }),
  };

  return (
    <>
      <ActionButton
        actions={reactionOverviewActions}
        size="xs"
        variant="outline"
      />
      {DeleteReactionState.isOpen && (
        <DeleteExperimentReactionModal
          isModalOpen={DeleteReactionState.isOpen}
          onModalClose={DeleteReactionState.onClose}
          reaction={reaction}
        />
      )}
      {EditReactionState.isOpen && (
        <CreateOrEditExperimentReactionModal
          isModalOpen={EditReactionState.isOpen}
          onModalClose={EditReactionState.onClose}
          reaction={reaction}
        />
      )}
    </>
  );
};
