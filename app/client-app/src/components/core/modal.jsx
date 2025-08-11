import {
  Button,
  Modal as ChakraModal,
  ModalBody,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
} from "@chakra-ui/react";
import { FaRegCheckCircle, FaTimes } from "react-icons/fa";

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
  contentMaxW,
  contentMaxH,
  bodyMaxH,
  bodyOverflowY,
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
    <ModalContent maxW={contentMaxW} maxH={contentMaxH}>
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
      <ModalBody maxH={bodyMaxH} overflowY={bodyOverflowY}>
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
