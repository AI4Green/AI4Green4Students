import { Avatar, Button, HStack, Text } from "@chakra-ui/react";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { FaPlus } from "react-icons/fa";

export const SectionFormAction = ({ record, isLoading, formRef }) => {
  const isInstructor = useIsInstructor();
  return (
    <HStack pb={1}>
      <Avatar name={record.ownerName} size="sm" />
      <Text fontSize="md" color="gray.600">
        {record.ownerName}
      </Text>
      {!isInstructor && (
        <Button
          colorScheme="green"
          leftIcon={<FaPlus />}
          size="xs"
          isLoading={isLoading}
          onClick={() => formRef.current.handleSubmit()}
        >
          <Text fontSize="xs" fontWeight="semibold">
            Save
          </Text>
        </Button>
      )}
    </HStack>
  );
};
