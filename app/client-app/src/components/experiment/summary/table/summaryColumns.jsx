import { Text, Icon, Flex, useDisclosure, Button } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import {
  TITLE_ICON_COMPONENTS,
  STATUS_ICON_COMPONENTS,
} from "constants/experiment-ui";
import { FaLink, FaPaperPlane, FaTrash } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { useState } from "react";
import { ActionButton } from "components/ActionButton";
import { DeleteModal } from "components/experiment/modal/DeleteModal";
import { MoveStageModal } from "components/experiment/modal/MoveStageModal";
import { useProjectSummaryByStudent } from "api/projects";
import { STAGES } from "constants/stages";
import { SECTION_TYPES } from "constants/section-types";

export const summaryColumns = (isInstructor) => [
  {
    accessorKey: "title",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Title" />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 2}>
        <Icon
          as={TITLE_ICON_COMPONENTS[row.original.dataType]}
          color="gray.600"
        />

        <Text
          fontWeight={(row.getCanExpand() || row.depth === 0) && "semibold"}
          fontSize={row.depth === 0 && "md"}
        >
          {cell.getValue()}
        </Text>
      </Flex>
    ),
  },
  {
    accessorKey: "stage",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 2}>
        <Icon
          as={STATUS_ICON_COMPONENTS[row.original.stage].icon}
          color={STATUS_ICON_COMPONENTS[row.original.stage].color}
        />

        <Text fontSize="xs">{cell.getValue()}</Text>
      </Flex>
    ),
  },

  {
    id: "notes",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Lab notes" />
    ),
    cell: ({ row }) => {
      const isPlanApproved = row.original.stage === STAGES.Approved;
      const NoteIcon = TITLE_ICON_COMPONENTS[SECTION_TYPES.Note];
      const targetPath = row.original.note?.targetPath;
      const navigate = useNavigate();
      return (
        <Button
          disabled={!isPlanApproved}
          size="xs"
          variant="outline"
          leftIcon={isPlanApproved && <NoteIcon />}
          onClick={() => navigate(targetPath)}
        >
          {isPlanApproved ? "View" : "Unavailable"}
        </Button>
      );
    },
  },

  ...((!isInstructor && [
    {
      id: "actions",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Actions" />
      ),
      cell: ({ row }) => <PlanAction plan={row.original} />,
    },
  ]) ||
    []),
];

const PlanAction = ({ plan }) => {
  const navigate = useNavigate();

  const {
    isOpen: isDeleteOpen,
    onOpen: onDeleteOpen,
    onClose: onDeleteClose,
  } = useDisclosure();

  const {
    isOpen: isAdvanceStageOpen,
    onOpen: onAdvanceStageOpen,
    onClose: onAdvanceStageClose,
  } = useDisclosure();

  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  const { mutate } = useProjectSummaryByStudent(plan.project.id);

  const actions = {
    view: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "View",
      onClick: () => navigate(plan.targetPath),
    },
    delete: {
      isEligible: () =>
        plan.permissions.includes(STAGES_PERMISSIONS.OwnerCanEdit),
      icon: <FaTrash />,
      label: "Delete",
      onClick: onDeleteOpen,
    },
    submit: {
      isEligible: () =>
        [
          STAGES_PERMISSIONS.OwnerCanEdit,
          STAGES_PERMISSIONS.OwnerCanEditCommented,
        ].some((permission) => plan.permissions.includes(permission)),
      icon: <FaPaperPlane />,
      label: plan.permissions.includes(STAGES_PERMISSIONS.OwnerCanEditCommented)
        ? "Submit changes"
        : "Submit",
      onClick: () => {
        setModalActionProps({
          modalTitle: `Submit Plan`,
          modalMessage:
            "Do you wish to proceed with submission of the following?",
          successMessage: "Plan submission succeeded",
          failMessage: `Plan submission failed`,
        });
        onAdvanceStageOpen();
      },
    },
  };

  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {isDeleteOpen && (
        <DeleteModal
          isModalOpen={isDeleteOpen}
          onModalClose={onDeleteClose}
          record={plan}
          sectionType={SECTION_TYPES.Plan}
        />
      )}
      {isAdvanceStageOpen && (
        <MoveStageModal
          isModalOpen={isAdvanceStageOpen}
          onModalClose={onAdvanceStageClose}
          record={plan}
          mutate={mutate}
          sectionType={SECTION_TYPES.Plan}
          {...modalActionProps}
        />
      )}
    </>
  );
};
