import { Text, Icon, IconButton, Flex, useDisclosure } from "@chakra-ui/react";
import {
  FaLayerGroup,
  FaChevronDown,
  FaChevronRight,
  FaLink,
  FaTrash,
} from "react-icons/fa";
import { CreateOrEditProjectModal as EditProjectModal } from "./modal/CreateOrEditProjectModal";
import { DeleteModal as DeleteProjectModal } from "./modal/DeleteModal";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { ActionButton } from "components/ActionButton";

export const ProjectColumns = [
  {
    id: "expander",
    cell: ({ row }) =>
      row.getCanExpand() ? (
        <IconButton
          size="xs"
          icon={row.getIsExpanded() ? <FaChevronRight /> : <FaChevronDown />}
          variant="ghost"
          onClick={() => row.toggleExpanded()}
          paddingLeft={row.depth * 3}
        />
      ) : null,
    enableHiding: false,
  },
  {
    accessorKey: "id",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="ID" />
    ),
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        {row.getCanExpand() && <Icon as={FaLayerGroup} color="green.600" />}
        <Text
          fontWeight={(row.getCanExpand() || row.depth === 0) && "semibold"}
        >
          {row.original.name}
        </Text>
      </Flex>
    ),
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="No. of Project Groups" />
    ),
    accessorKey: "projectGroupNumber",
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="No. of Students" />
    ),
    accessorKey: "studentNumber",
    cell: ({ row }) => {
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      const totalStudents =
        isParent && row.subRows.length > 0
          ? row.original.subRows.reduce(
              (total, subRow) => total + subRow.studentNumber,
              0
            )
          : row.original.studentNumber;

      return (
        <Text fontWeight={(isParent || row.depth === 0) && "semibold"}>
          {totalStudents}
        </Text>
      );
    },
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Action" />
    ),
    cell: ({ row }) => {
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      return isParent && <ProjectAction project={row.original} />;
    },
  },
];

const ProjectAction = ({ project }) => {
  const EditProjectState = useDisclosure();
  const DeleteProjectState = useDisclosure();

  const projectManagementActions = {
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: EditProjectState.onOpen,
    },
    delete: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Delete",
      onClick: DeleteProjectState.onOpen,
    },
  };
  return (
    <>
      <ActionButton
        actions={projectManagementActions}
        size="xs"
        variant="outline"
      />
      {EditProjectState.isOpen && (
        <EditProjectModal
          isModalOpen={EditProjectState.isOpen}
          onModalClose={EditProjectState.onClose}
          project={project}
        />
      )}
      {DeleteProjectState.isOpen && (
        <DeleteProjectModal
          isModalOpen={DeleteProjectState.isOpen}
          onModalClose={DeleteProjectState.onClose}
          project={project}
        />
      )}
    </>
  );
};
