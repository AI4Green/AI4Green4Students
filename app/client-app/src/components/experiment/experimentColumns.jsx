import { Text, Icon, Flex, useDisclosure } from "@chakra-ui/react";
import { FaFlask, FaTrash, FaLink, FaLayerGroup } from "react-icons/fa";
import { DeleteModal as DeleteExperimentModal } from "./modal/DeleteModal";
import { CreateOrEditExperimentModal as EditExperimentModal } from "./modal/CreateOrEditExperimentModal";
import { useNavigate } from "react-router-dom";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { ActionButton } from "components/ActionButton";
import { DataTableRowExpander } from "components/dataTable/DataTableRowExpander";

export const experimentColumns = (isInstructor) => [
  {
    id: "expander",
    cell: ({ row }) => <DataTableRowExpander row={row} />,
    enableHiding: false,
  },
  {
    accessorKey: "id",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="ID" />
    ),
    enableHiding: false,
  },
  ...(isInstructor
    ? [
        {
          accessorKey: "studentName",
          header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Student name" />
          ),
          enableHiding: false,
        },
      ]
    : []),
  {
    accessorKey: "title",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Title" />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 2}>
        {(row.original.isOverview || row.original.isReaction) && (
          <Icon
            as={row.original.isOverview ? FaLayerGroup : FaFlask}
            color="green.600"
          />
        )}

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
    accessorKey: "status",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
  },
  {
    accessorKey: isInstructor ? "projectGroups" : "project",
    header: ({ column }) => (
      <DataTableColumnHeader
        column={column}
        title={isInstructor ? "Project group" : "Project"}
      />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        <Text>{cell.getValue()?.name} </Text>
      </Flex>
    ),
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Action" />
    ),
    cell: ({ row }) => {
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      return isParent ? (
        !isInstructor && <ExperimentAction row={row} /> // show action buttons for student
      ) : row.original.isOverview ? (
        <OverviewAction experiment={row.original} isInstructor={isInstructor} />
      ) : (
        <OverviewAction
          experiment={row.original}
          isReaction
          isInstructor={isInstructor}
        />
      );
    },
  },
];

const ExperimentAction = ({ row }) => {
  const DeleteExperimentState = useDisclosure();
  const EditExperimentState = useDisclosure();

  const experimentActions = {
    expandCollapse: {
      isEligible: () => true,
      icon: <FaLink />,
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
        <EditExperimentModal
          isModalOpen={EditExperimentState.isOpen}
          onModalClose={EditExperimentState.onClose}
          experiment={row.original}
          projectGroup={row.original.project.projectGroups[0]}
        />
      )}
    </>
  );
};

const OverviewAction = ({ experiment, isReaction, isInstructor }) => {
  const navigate = useNavigate();
  const link = isReaction
    ? `/experiments/${experiment.experimentId}/reaction-overview/${experiment.reactionId}`
    : `/experiments/${experiment.experimentId}/plan-overview`;

  return (
    <ActionButton
      actions={{
        edit: {
          isEligible: () => true,
          icon: <FaLink />,
          label: isInstructor ? "View" : "Edit",
          onClick: () => navigate(link),
        },
      }}
      size="xs"
      variant="outline"
    />
  );
};
