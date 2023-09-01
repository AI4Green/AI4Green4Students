import {
  HStack,
  Text,
  Button,
  Icon,
  Flex,
  useDisclosure,
} from "@chakra-ui/react";
import { FaProjectDiagram, FaLink, FaTrash } from "react-icons/fa";
import { ModalCreateOrEditProjectGroup as ModalEditProjectGroup } from "./modal/ModalCreateOrEditProjectGroup";
import { ModalDelete as ModalDeleteProjectGroup } from "./modal/ModalDelete";

export const ProjectGroupColumns = [
  {
    Header: "Id",
    accessor: "id",
  },
  {
    Header: "Name",
    accessor: "name",
    Cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        {<Icon as={FaProjectDiagram} color="green.600" />}
        <Text fontWeight="semibold">{row.original.name}</Text>
      </Flex>
    ),
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
    Cell: ({ row }) => (
      <ProjectGroupActions key={row.original.id} group={row.original} />
    ),
  },
];

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
        <ModalEditProjectGroup
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
        <ModalDeleteProjectGroup
          isModalOpen={DeleteProjectGroupState.isOpen}
          onModalClose={DeleteProjectGroupState.onClose}
          projectGroup={group}
        />
      )}
    </HStack>
  );
};
