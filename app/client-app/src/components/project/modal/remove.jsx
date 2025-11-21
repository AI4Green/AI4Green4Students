import { Alert, AlertIcon, HStack, Text, VStack } from "@chakra-ui/react";
import { Badge } from "components/core/Badge";

export const RemoveModal = ({
  title = "Please confirm the removal of the following:",
  remove,
  tags,
  feedback,
}) => {
  return (
    <VStack align="flex-start" spacing={8}>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}

      <VStack align="flex-start" spacing={4}>
        <VStack align="flex-start" spacing={2}>
          {title && (
            <Text fontSize="sm" fontWeight="light" color="gray.600">
              {title}
            </Text>
          )}
          {remove && (
            <Text fontSize="sm" fontWeight="bold" color="gray.600">
              {remove}
            </Text>
          )}
        </VStack>
        {tags?.length > 0 && (
          <HStack>
            {tags.map((tag) => (
              <Badge
                key={tag.label}
                colorScheme={tag.colorScheme}
                label={tag.label}
                leftIcon={tag.leftIcon}
                variant="outline"
                fontSize="xxs"
              />
            ))}
          </HStack>
        )}
      </VStack>
    </VStack>
  );
};
