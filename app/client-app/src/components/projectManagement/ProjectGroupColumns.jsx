import {
  HStack,
  Text,
  Button,
  Icon,
  IconButton,
  Flex,
  useDisclosure,
} from "@chakra-ui/react";
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

export const ProjectGroupColumns = [
  {
    id: "expander",
    Cell: ({ row }) =>
      row.canExpand ? (
        <IconButton
          size="xs"
          icon={row.isExpanded ? <FaChevronRight /> : <FaChevronDown />}
          variant="ghost"
          onClick={() => row.toggleRowExpanded()}
          paddingLeft={row.depth * 3}
        />
      ) : null,
  },
  {
    Header: "Id",
    accessor: "id",
  },
  {
    Header: "Name",
    accessor: "name",
    Cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        {row.canExpand && <Icon as={FaProjectDiagram} color="green.600" />}
        <Text fontWeight={(row.canExpand || row.depth === 0) && "semibold"}>
          {row.original.name}
        </Text>
      </Flex>
    ),
  },
  {
    Header: "Student email",
    accessor: "studentEmail",
  },
  {
    Header: "Project",
    accessor: "projectName",
  },
  {
    Header: "No. of Students",
    accessor: "studentNumber",
  },

  {
    Header: "Actions",
    accessor: "actions",
    Cell: ({ row, rowsById }) => {
      const isParent = row.depth === 0;
      const parentRowId = row.id.split(".").slice(0, -1).join(".");
      const parentRow = rowsById[parentRowId];
      return isParent ? (
        <ProjectGroupActions group={row.original} />
      ) : (
        <ProjectGroupStudentActions
          student={row.original}
          group={parentRow.original}
        />
      );
    },
  },
];

const ProjectGroupStudentActions = ({ student, group }) => {
  const RemoveStudentState = useDisclosure();
  return (
    <HStack spacing={2}>
      <Button
        size="xs"
        variant="outline"
        colorScheme="red"
        leftIcon={<FaTrash />}
        onClick={RemoveStudentState.onOpen}
      >
        Remove
      </Button>
      {RemoveStudentState.isOpen && (
        <RemoveStudentModal
          isModalOpen={RemoveStudentState.isOpen}
          onModalClose={RemoveStudentState.onClose}
          student={student}
          projectGroup={group}
        />
      )}
    </HStack>
  );
};

const ProjectGroupActions = ({ group }) => {
  const EditProjectGroupState = useDisclosure();
  const DeleteProjectGroupState = useDisclosure();
  return (
    <HStack spacing={2}>
      <Button
        size="xs"
        variant="outline"
        colorScheme="blue"
        leftIcon={<FaLink />}
        onClick={EditProjectGroupState.onOpen}
      >
        Edit
      </Button>
      {EditProjectGroupState.isOpen && (
        <EditProjectGroupModal
          isModalOpen={EditProjectGroupState.isOpen}
          onModalClose={EditProjectGroupState.onClose}
          projectGroup={group}
        />
      )}
      <Button
        size="xs"
        variant="outline"
        colorScheme="red"
        leftIcon={<FaTrash />}
        onClick={DeleteProjectGroupState.onOpen}
      >
        Delete
      </Button>
      {DeleteProjectGroupState.isOpen && (
        <DeleteProjectGroupModal
          isModalOpen={DeleteProjectGroupState.isOpen}
          onModalClose={DeleteProjectGroupState.onClose}
          projectGroup={group}
        />
      )}
    </HStack>
  );
};
