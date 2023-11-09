import { Text, Icon, Flex, useDisclosure } from "@chakra-ui/react";
import { FaFlask, FaTrash, FaLink } from "react-icons/fa";
import { DeleteModal as DeleteExperimentModal } from "./modal/DeleteModal";
import { useNavigate } from "react-router-dom";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { ActionButton } from "components/ActionButton";

export const ExperimentColumns = [
  {
    accessorKey: "id",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="ID" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "title",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Title" />
    ),
    cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        <Icon as={FaFlask} color="green.600" />
        <Text fontWeight="semibold">{row.original.title}</Text>
      </Flex>
    ),
  },
  {
    accessorKey: "project",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Project" />
    ),
    cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        <Text>{row.original.project} </Text>
      </Flex>
    ),
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Action" />
    ),
    cell: ({ row }) => <ExperimentAction experiment={row.original} />,
  },
];

const ExperimentAction = ({ experiment }) => {
  const navigate = useNavigate();
  const DeleteExperimentState = useDisclosure();
  const experimentActions = {
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: () => navigate(`/experiments/${experiment.id}/planoverview`),
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
          experiment={experiment}
        />
      )}
    </>
  );
};
