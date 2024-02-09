import { Box, HStack, Text, VStack } from "@chakra-ui/react";
import { format, parseISO } from "date-fns";
import { CommentActions } from "./CommentActions";

export const CommentLog = ({ comment, fieldResponseId }) => {
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
        <Text fontWeight="semibold">{comment.owner}</Text>
        <Text>{formattedDate(comment.commentDate)}</Text>
      </HStack>
    </VStack>
  );
};

const formattedDate = (dateString) => {
  const date = parseISO(dateString);
  return format(date, "dd-MM-yyyy HH:mm:ss");
};
