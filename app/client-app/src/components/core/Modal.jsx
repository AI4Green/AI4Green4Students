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
  size,
  title,
  body,
  isOpen,
  onClose,
  onAction,
  isLoading,
  actionBtnCaption = "Ok",
  actionBtnColorScheme = "green",
  actionBtnLeftIcon = <FaRegCheckCircle />,
  cancelBtnEnable = true,
  cancelBtnCaption = "Cancel",
  cancelBtnAction = onClose,
  closeOnOverlayClick = true,
}) => (
  <ChakraModal
    closeOnEsc={closeOnOverlayClick}
    closeOnOverlayClick={closeOnOverlayClick}
    isOpen={isOpen}
    onClose={cancelBtnAction}
    size={size}
    isCentered
  >
    <ModalOverlay />
    <ModalContent maxW="80vw" maxH="90vh">
      <ModalHeader fontSize="lg" fontWeight="bold">
        <Button
          onClick={cancelBtnAction}
          leftIcon={<FaTimes />}
          variant="ghost"
          size="sm"
          float="right"
        />

        {title}
      </ModalHeader>
      <ModalBody overflowY="auto" maxH="75vh">
        {body}
      </ModalBody>
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
