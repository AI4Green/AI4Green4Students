import {
  HStack,
  Text,
  Button,
  Icon,
  Flex,
  useDisclosure,
} from "@chakra-ui/react";
import { FaLayerGroup, FaTrash } from "react-icons/fa";
import { DeleteModal as DeleteExperimentModal } from "./modal/DeleteModal";

export const ExperimentColumns = [
  {
    Header: "Id",
    accessor: "id",
  },
  {
    Header: "Title",
    accessor: "title",
    Cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        <Text fontWeight="semibold">{row.original.title}</Text>
      </Flex>
    ),
  },
  {
    Header: "Project",
    accessor: "project",
    Cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        <Icon as={FaLayerGroup} color="green.600" />
        <Text>{row.original.project} </Text>
      </Flex>
    ),
  },
  {
    Header: "Actions",
    accessor: "actions",
    Cell: ({ row }) => <ExperimentAction experiment={row.original} />,
  },
];

const ExperimentAction = ({ experiment }) => {
  const DeleteExperimentState = useDisclosure();
  return (
    <HStack spacing={2}>
      <Button
        size="xs"
        variant="outline"
        colorScheme="red"
        leftIcon={<FaTrash />}
        onClick={DeleteExperimentState.onOpen}
      >
        Delete
      </Button>
      {DeleteExperimentState.isOpen && (
        <DeleteExperimentModal
          isModalOpen={DeleteExperimentState.isOpen}
          onModalClose={DeleteExperimentState.onClose}
          experiment={experiment}
        />
      )}
    </HStack>
  );
};
