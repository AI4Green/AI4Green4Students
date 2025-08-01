import { Box, HStack, Text, VStack } from "@chakra-ui/react";
import { CommentActions } from ".";
import { getFormattedDate } from "helpers";
import { useUser } from "contexts";

export const CommentLog = ({ comment, fieldResponseId }) => {
  const { user } = useUser();
  return (
    <VStack
      align="stretch"
      borderBottomWidth={1}
      borderRadius={5}
      p={1}
      my={2}
      fontSize="sm"
      bgColor={!comment.read && "gray.100"}
    >
      <Box display="flex" justifyContent="flex-end" mb={-3}>
        <CommentActions comment={comment} fieldResponseId={fieldResponseId} />
      </Box>

      <Text>{comment.value}</Text>
      <HStack fontSize="xs" justify="flex-end">
        <Text fontWeight="medium">{comment.owner}</Text>
        <Text>{getFormattedDate(comment.commentDate, user.uiCulture)}</Text>
      </HStack>
    </VStack>
  );
};
