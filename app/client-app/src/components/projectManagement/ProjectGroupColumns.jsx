import { Text, Icon, IconButton, Flex, useDisclosure } from "@chakra-ui/react";
import {
  FaProjectDiagram,
  FaLink,
  FaTrash,
  FaChevronDown,
  FaChevronRight,
} from "react-icons/fa";
import { CreateOrEditProjectGroupModal as EditProjectGroupModal } from "./modal/CreateOrEditProjectGroupModal";
import { DeleteModal as DeleteProjectGroupModal } from "./modal/DeleteModal";
import { RemoveStudentModal } from "./modal/RemoveStudentModal";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { ActionButton } from "components/ActionButton";

export const ProjectGroupColumns = [
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
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="ID" />
    ),
    accessorKey: "id",
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        {row.getCanExpand() && <Icon as={FaProjectDiagram} color="green.600" />}
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
      <DataTableColumnHeader column={column} title="Student email" />
    ),
    accessorKey: "studentEmail",
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Project" />
    ),
    accessorKey: "projectName",
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="No. of Students" />
    ),
    accessorKey: "studentNumber",
  },
  {
    id: "actions",
    cell: ({ row }) => {
      const isParent = row.depth === 0;
      const parentRowId = row.id.split(".").slice(0, -1).join(".");
      const parentRow = row.getParentRow(parentRowId);
      return isParent ? (
        <ProjectGroupAction group={row.original} />
      ) : (
        <ProjectGroupStudentAction
          student={row.original}
          group={parentRow.original}
        />
      );
    },
  },
];

const ProjectGroupStudentAction = ({ student, group }) => {
  const RemoveStudentState = useDisclosure();
  const projectGroupStudentActions = {
    remove: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Remove",
      onClick: RemoveStudentState.onOpen,
      colorScheme: "red",
    },
  };
  return (
    <>
      <ActionButton
        actions={projectGroupStudentActions}
        size="xs"
        variant="outline"
      />
      {RemoveStudentState.isOpen && (
        <RemoveStudentModal
          isModalOpen={RemoveStudentState.isOpen}
          onModalClose={RemoveStudentState.onClose}
          student={student}
          projectGroup={group}
        />
      )}
    </>
  );
};

const ProjectGroupAction = ({ group }) => {
  const EditProjectGroupState = useDisclosure();
  const DeleteProjectGroupState = useDisclosure();
  const projectGroupActions = {
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: EditProjectGroupState.onOpen,
    },
    delete: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Delete",
      onClick: DeleteProjectGroupState.onOpen,
    },
  };
  return (
    <>
      <ActionButton actions={projectGroupActions} size="xs" variant="outline" />
      {EditProjectGroupState.isOpen && (
        <EditProjectGroupModal
          isModalOpen={EditProjectGroupState.isOpen}
          onModalClose={EditProjectGroupState.onClose}
          projectGroup={group}
        />
      )}

      {DeleteProjectGroupState.isOpen && (
        <DeleteProjectGroupModal
          isModalOpen={DeleteProjectGroupState.isOpen}
          onModalClose={DeleteProjectGroupState.onClose}
          projectGroup={group}
        />
      )}
    </>
  );
};
