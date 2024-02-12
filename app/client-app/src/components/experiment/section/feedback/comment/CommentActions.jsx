import { IconButton, useDisclosure, useToast } from "@chakra-ui/react";
import { FaEdit, FaRegDotCircle, FaRegTimesCircle } from "react-icons/fa";
import { CreateOrEditCommentModal } from "./modal/CreateOrEditCommentModal";
import { DeleteCommentModal } from "./modal/DeleteCommentModal";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { useBackendApi } from "contexts/BackendApi";
import { useSectionForm } from "contexts/SectionForm";

export const CommentActions = ({ comment, fieldResponseId }) => {
  const { mutate } = useSectionForm();
  const { comments: action } = useBackendApi();
  const isInstructor = useIsInstructor();
  const toast = useToast();

  const editState = useDisclosure();
  const deleteState = useDisclosure();

  const handleMarkCommentAsRead = async () => {
    try {
      const response = await action.markAsRead(comment.id);
      if (response && response.status === 204) {
        toast(toastOptions("Comment marked as read", "success"));
        await mutate();
      }
    } catch (e) {
      toast(toastOptions("Error", "error"));
    }
  };

  return (
    <>
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
      )}
    </>
  );
};

const toastOptions = (title, status) => ({
  position: "top",
  title,
  status,
  duration: 1500,
  isClosable: true,
});
