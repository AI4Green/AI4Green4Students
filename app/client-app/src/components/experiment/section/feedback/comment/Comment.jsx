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
} from "@chakra-ui/react";
import { FaRegCommentAlt } from "react-icons/fa";
import { NotificationBadge } from "components/NotificationBadge";
import { useBackendApi } from "contexts/BackendApi";
import { useState } from "react";
import { LoadingIndicator } from "components/LoadingIndicator";
import { CommentLog } from "./CommentLog";

export const Comment = ({ field }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [feedback, setFeedback] = useState();
  const [commentLogs, setCommentLogs] = useState([]);

  const { comments: action } = useBackendApi();
  const unreadComments = field.comments || 0;

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
