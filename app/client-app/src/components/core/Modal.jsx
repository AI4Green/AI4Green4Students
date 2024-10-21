import {
  Button,
  Modal as ChakraModal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
} from "@chakra-ui/react";
import { FaTimes, FaRegCheckCircle } from "react-icons/fa";

export const Modal = ({
  title, // Modal title
  body,
  isOpen,
  onClose,
  onAction, // onClick event action for Ok/primary button
  isLoading,
  actionBtnCaption = "Ok", // caption for the primary button
  actionBtnColorScheme = "green", // color scheme for the primary button
  actionBtnLeftIcon = <FaRegCheckCircle />,
  cancelBtnEnable = true, // By default, Cancel/Secondary button is visible/enabled
  cancelBtnCaption = "Cancel", // caption for the Cancel/Secondary button
  cancelBtnAction = onClose,
  closeOnOverlayClick = true,
}) => (
  <ChakraModal
    closeOnEsc={closeOnOverlayClick}
    closeOnOverlayClick={closeOnOverlayClick}
    isOpen={isOpen}
    onClose={cancelBtnAction}
  >
    <ModalOverlay />
    <ModalContent>
      <ModalHeader fontSize="lg" fontWeight="bold">
        {title}
      </ModalHeader>
      <ModalBody>{body}</ModalBody>
      <ModalFooter>
        {cancelBtnEnable && (
          <Button onClick={cancelBtnAction} leftIcon={<FaTimes />}>
            {cancelBtnCaption}
          </Button>
        )}
        <Button
          leftIcon={actionBtnLeftIcon}
          colorScheme={actionBtnColorScheme}
          onClick={onAction}
          ml={3}
          isLoading={isLoading}
        >
          {actionBtnCaption}
        </Button>
      </ModalFooter>
    </ModalContent>
  </ChakraModal>
);
