import {
  HStack,
  Text,
  Button,
  Icon,
  Flex,
  useDisclosure,
} from "@chakra-ui/react";
import { FaFlask, FaTrash, FaLink } from "react-icons/fa";
import { DeleteModal as DeleteExperimentModal } from "./modal/DeleteModal";
import { useNavigate } from "react-router-dom";

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
        <Icon as={FaFlask} color="green.600" />
        <Text fontWeight="semibold">{row.original.title}</Text>
      </Flex>
    ),
  },
  {
    Header: "Project",
    accessor: "project",
    Cell: ({ row }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
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
  const navigate = useNavigate();
  const DeleteExperimentState = useDisclosure();
  return (
    <HStack spacing={2}>
      <Button
        size="xs"
        variant="outline"
        colorScheme="blue"
        leftIcon={<FaLink />}
        onClick={() =>
          navigate(
            `/experiments/project/${experiment.projectId}/planoverview/${experiment.id}`
          )
        }
      >
        Edit
      </Button>
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
