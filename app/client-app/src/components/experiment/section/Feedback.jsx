import {
  Tag,
  TagLabel,
  TagCloseButton,
  useDisclosure,
  TagLeftIcon,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useState } from "react";
import { ActionButton } from "components/ActionButton";
import { FaRegCommentAlt, FaCheck } from "react-icons/fa";
import { CreateCommentModal } from "../modal/CreateCommentModal";
import { Comment } from "./Comment";
import { LoadingIndicator } from "components/LoadingIndicator";

export const Feedback = ({ field, isInstructor }) => {
  // field.comments is the number of comments

  // also expecting following properties to be present in field object
  // isApproved: boolean. denotes whether the field response has been approved or not

  // Probably the following as well
  // hasBeenReviewed: boolean. denotes whether the field response has been reviewed or not
  // hasBeenSubmitted: boolean. denotes field response has been submitted to be reviewed.

  const handleRemoveApproval = async () => {
    // TODO: make an api call to set field response as not approved
  };

  return (
    <VStack align="flex-start">
      {field.isApproved ? (
        <Tag colorScheme="green">
          <TagLeftIcon as={FaCheck} />
          <TagLabel>Approved</TagLabel>
          {isInstructor && <TagCloseButton onClick={handleRemoveApproval} />}
        </Tag>
      ) : (
        isInstructor && <FeedbackActionsMenu field={field} />
      )}
      {field.comments >= 1 && (
        <Comment field={field} canMarkCommentAsRead={!isInstructor} />
      )}
    </VStack>
  );
};

const FeedbackActionsMenu = ({ field }) => {
  const AddCommentState = useDisclosure();
  const [isLoading, setIsLoading] = useState(false);
  const toast = useToast();

  const handleApproval = async () => {
    // TODO: make api call to set field-response as approved, also set the field-response as reviewed
    // display loading indicator while api call is in progress
    // display toast message on success or error
  };

  // set of actions, which will be passed to ActionButton component as props
  const actions = {
    approve: {
      isEligible: () => true,
      label: "Approve",
      onClick: () => handleApproval(),
      icon: <FaCheck />,
    },
    comment: {
      isEligible: () => true,
      label: "Add comment",
      onClick: AddCommentState.onOpen,
      icon: <FaRegCommentAlt />,
    },
  };
  return isLoading ? (
    <LoadingIndicator verb />
  ) : (
    <>
      <ActionButton actions={actions} size="xs" variant="outline" />
      {AddCommentState.isOpen && (
        <CreateCommentModal
          // this might change as instead of passing the whole field object as prop,
          // we might just pass fieldResponseId, which will be used to make api call to add a comment for that field response
          field={field}
          isModalOpen={AddCommentState.isOpen}
          onModalClose={AddCommentState.onClose}
        />
      )}
    </>
  );
};
