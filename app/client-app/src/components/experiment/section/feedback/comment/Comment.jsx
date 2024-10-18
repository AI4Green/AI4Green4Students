import {
  Popover,
  PopoverTrigger,
  Portal,
  PopoverContent,
  PopoverArrow,
  PopoverHeader,
  PopoverCloseButton,
  PopoverBody,
  Box,
  Text,
} from "@chakra-ui/react";
import { FaRegCommentDots } from "react-icons/fa";
import { NotificationBadge } from "components/core/NotificationBadge";
import { useBackendApi } from "contexts/BackendApi";
import { useState } from "react";
import { LoadingIndicator } from "components/core/LoadingIndicator";
import { CommentLog } from "./CommentLog";

export const Comment = ({ field }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [feedback, setFeedback] = useState();
  const [commentLogs, setCommentLogs] = useState([]);

  const { comments: action } = useBackendApi();
  const unreadComments = field.unreadComments || 0;

  const handleOpen = async () => {
    try {
      setIsLoading(true);
      const data = await action.getCommentLogs(field.fieldResponseId);
      setCommentLogs(data);
      feedback && setFeedback(null);
    } catch (error) {
      console.error(error);
      setFeedback("Error fetching comments");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Popover onOpen={handleOpen}>
      <PopoverTrigger>
        <Box>
          <NotificationBadge
            as="button"
            type="button"
            count={unreadComments}
            icon={FaRegCommentDots}
            aria-label="Comments"
          />
        </Box>
      </PopoverTrigger>
      <Portal>
        <PopoverContent>
          <PopoverArrow />
          <PopoverHeader>Comments</PopoverHeader>
          <PopoverCloseButton />
          <PopoverBody overflowY="auto" maxH="300px">
            {isLoading ? (
              <LoadingIndicator />
            ) : feedback ? (
              <Text color="red">{feedback}</Text>
            ) : (
              commentLogs
                ?.sort(
                  (a, b) => new Date(b.commentDate) - new Date(a.commentDate)
                )
                .map((log) => (
                  <CommentLog
                    key={log.id}
                    comment={log}
                    fieldResponseId={field.fieldResponseId}
                  />
                ))
            )}
          </PopoverBody>
        </PopoverContent>
      </Portal>
    </Popover>
  );
};
