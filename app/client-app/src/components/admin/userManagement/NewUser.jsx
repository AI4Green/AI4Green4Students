import { Text, Button, useDisclosure } from "@chakra-ui/react";
import { FaPlus } from "react-icons/fa";
import { ManageRolesOrInviteModal } from "./modal/ManageRolesOrInviteModal";

export const NewUser = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <>
      <Button
        onClick={onOpen}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          New user
        </Text>
      </Button>

      <ManageRolesOrInviteModal isModalOpen={isOpen} onModalClose={onClose} />
    </>
  );
};
