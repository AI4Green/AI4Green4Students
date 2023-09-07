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
  FaLayerGroup,
  FaChevronDown,
  FaChevronRight,
  FaLink,
  FaTrash,
} from "react-icons/fa";
import { CreateOrEditProjectModal as EditProjectModal } from "./modal/CreateOrEditProjectModal";
import { DeleteModal as DeleteProjectModal } from "./modal/DeleteModal";

export const ProjectColumns = [
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
        {row.canExpand && <Icon as={FaLayerGroup} color="green.600" />}
        <Text fontWeight={(row.canExpand || row.depth === 0) && "semibold"}>
          {row.original.name}
        </Text>
      </Flex>
    ),
  },
  {
    Header: "No. of Project Groups",
    accessor: "projectGroupNumber",
  },
  {
    Header: "No. of Students",
    accessor: "studentNumber",
    Cell: ({ row }) => {
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      const totalStudents =
        isParent && row.subRows.length > 0
          ? row.subRows.reduce(
              (total, subRow) => total + subRow.values.studentNumber,
              0
            )
          : row.values.studentNumber;

      return (
        <Text fontWeight={(isParent || row.depth === 0) && "semibold"}>
          {totalStudents}
        </Text>
      );
    },
  },

  {
    Header: "Actions",
    accessor: "actions",
    Cell: ({ row }) => {
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      return isParent && <ProjectManagementAction project={row.original} />;
    },
  },
];

const ProjectManagementAction = ({ project }) => {
  const EditProjectState = useDisclosure();
  const DeleteProjectState = useDisclosure();
  return (
    <HStack spacing={2}>
      <Button
        size="xs"
        variant="outline"
        colorScheme="blue"
        leftIcon={<FaLink />}
        onClick={EditProjectState.onOpen}
      >
        Edit
      </Button>
      {EditProjectState.isOpen && (
        <EditProjectModal
          isModalOpen={EditProjectState.isOpen}
          onModalClose={EditProjectState.onClose}
          project={project}
        />
      )}
      <Button
        size="xs"
        variant="outline"
        colorScheme="red"
        leftIcon={<FaTrash />}
        onClick={DeleteProjectState.onOpen}
      >
        Delete
      </Button>
      {DeleteProjectState.isOpen && (
        <DeleteProjectModal
          isModalOpen={DeleteProjectState.isOpen}
          onModalClose={DeleteProjectState.onClose}
          project={project}
        />
      )}
    </HStack>
  );
};
