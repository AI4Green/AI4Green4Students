import {
  Popover,
  PopoverTrigger,
  Portal,
  PopoverContent,
  PopoverArrow,
  PopoverHeader,
  PopoverCloseButton,
  PopoverBody,
  IconButton,
  Box,
  Text,
  HStack,
  VStack,
  useToast,
} from "@chakra-ui/react";
import { FaRegCommentAlt, FaRegDotCircle } from "react-icons/fa";
import { NotificationBadge } from "components/NotificationBadge";
import { useComments } from "api/comment";
import { format, parse, parseISO } from "date-fns";

export const Comment = ({ field, canMarkCommentAsRead = true }) => {
  // comment icon is rendered based on the number of unread comments, which can be either retrieved from the field object
  // or from the api call
  // if retrieving from the field object, then it's straightforward
  // if making an api call then, we might retrieve the unread comments count and the comments logs in the same api call

  // for now, we assume that the unread comments count and comment logs are retrieved from the field object
  const unreadComments = field.comments ?? 0;
  const { data: commentLogs } = useComments(field.fieldResponseId);

  return (
    <Popover>
      <PopoverTrigger>
        {unreadComments >= 1 ? (
          <Box>
            <NotificationBadge count={unreadComments} icon={FaRegCommentAlt} />
          </Box>
        ) : (
          <IconButton
            aria-label="Comments"
            icon={<FaRegCommentAlt />}
            size="lg"
            variant="ghost"
          />
        )}
      </PopoverTrigger>
      <Portal>
        <PopoverContent>
          <PopoverArrow />
          <PopoverHeader fontWeight="bold">Comment logs</PopoverHeader>
          <PopoverCloseButton />
          <PopoverBody overflowY="auto" maxH="300px">
            {commentLogs?.map((log) => (
              <CommentLog
                key={log.id}
                comment={log}
                canMarkCommentAsRead={canMarkCommentAsRead}
              />
            ))}
          </PopoverBody>
        </PopoverContent>
      </Portal>
    </Popover>
  );
};

const CommentLog = ({ comment, canMarkCommentAsRead }) => {
  const toast = useToast();

  const handleMarkCommentAsRead = async () => {
    // TODO: make api call to mark the comment as read
    // display toast notification on success
  };

  return (
    <VStack
      align="stretch"
      borderBottomWidth={1}
      borderRadius={5}
      p={1}
      my={2}
      fontSize="sm"
      bgColor={!comment.isRead && "gray.100"}
    >
      {!comment.isRead && canMarkCommentAsRead && (
        <Box display="flex" justifyContent="flex-end" mb={-3}>
          <IconButton
            icon={<FaRegDotCircle />}
            isRound
            size="xs"
            variant="ghost"
            onClick={handleMarkCommentAsRead}
          />
        </Box>
      )}

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
  return format(date, "dd-MM-yyyy");
};
