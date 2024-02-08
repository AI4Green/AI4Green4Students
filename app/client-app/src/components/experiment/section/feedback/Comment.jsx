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
  useDisclosure,
} from "@chakra-ui/react";
import {
  FaEdit,
  FaRegCommentAlt,
  FaRegDotCircle,
  FaRegTimesCircle,
  FaRemoveFormat,
} from "react-icons/fa";
import { NotificationBadge } from "components/NotificationBadge";
import { useComments } from "api/comment";
import { format, parseISO } from "date-fns";
import { useBackendApi } from "contexts/BackendApi";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { CreateOrEditCommentModal } from "./modal/CreateOrEditCommentModal";
import { DeleteCommentModal } from "./modal/DeleteCommentModal";

export const Comment = ({ field }) => {
  const { data: commentLogs, mutate } = useComments(field.fieldResponseId);
  const unreadComments = commentLogs?.filter((log) => !log.read).length || 0;

  if (!commentLogs?.length >= 1) return null;

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
          <PopoverHeader fontWeight="bold">Comments</PopoverHeader>
          <PopoverCloseButton />
          <PopoverBody overflowY="auto" maxH="300px">
            {commentLogs
              ?.sort(
                (a, b) => new Date(b.commentDate) - new Date(a.commentDate)
              )
              .map((log) => (
                <CommentLog
                  key={log.id}
                  comment={log}
                  mutate={mutate}
                  fieldResponseId={field.fieldResponseId}
                />
              ))}
          </PopoverBody>
        </PopoverContent>
      </Portal>
    </Popover>
  );
};

const CommentLog = ({ comment, mutate, fieldResponseId }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const toast = useToast();

  const { comments: action } = useBackendApi();
  const isInstructor = useIsInstructor();

  const handleMarkCommentAsRead = async () => {
    try {
      const response = await action.markAsRead(comment.id);
      if (response && response.status === 204) {
        toast({
          position: "top",
          title: "Comment marked as read",
          status: "success",
          duration: 1500,
          isClosable: true,
        });
        mutate();
      }
    } catch (e) {
      toast({
        position: "top",
        title: "Error",
        status: "error",
        duration: 1500,
        isClosable: true,
      });
    }
  };

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
        {!comment.read && !isInstructor && (
          <IconButton
            icon={<FaRegDotCircle />}
            isRound
            size="xs"
            variant="ghost"
            onClick={handleMarkCommentAsRead}
          />
        )}
        {isInstructor && (
          <InstructorActions
            comment={comment}
            fieldResponseId={fieldResponseId}
          />
        )}
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

const InstructorActions = ({ comment, fieldResponseId }) => {
  const editState = useDisclosure();
  const deleteState = useDisclosure();

  return (
    <>
      <IconButton
        icon={<FaEdit />}
        isRound
        size="xs"
        variant="ghost"
        onClick={editState.onOpen}
      />
      {editState.isOpen && (
        <CreateOrEditCommentModal
          comment={comment}
          fieldResponseId={fieldResponseId}
          isModalOpen={editState.isOpen}
          onModalClose={editState.onClose}
        />
      )}

      <IconButton
        icon={<FaRegTimesCircle />}
        isRound
        size="xs"
        variant="ghost"
        onClick={deleteState.onOpen}
      />
      {deleteState.isOpen && (
        <DeleteCommentModal
          comment={comment}
          fieldResponseId={fieldResponseId}
          isModalOpen={deleteState.isOpen}
          onModalClose={deleteState.onClose}
        />
      )}
    </>
  );
};
