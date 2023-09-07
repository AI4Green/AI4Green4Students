import { Alert, AlertIcon, VStack, Text, Checkbox } from "@chakra-ui/react";

export const DeleteUserModal = ({
  feedback,
  user,
  setInformUser,
  informUser,
}) => {
  return (
    <VStack>
      {feedback && (
        <Alert status="error">
          <AlertIcon />
          {feedback}
        </Alert>
      )}
      <Text>Are you sure you want to delete this user:</Text>
      <Text fontWeight="bold">{user.fullName || user.email}</Text>
      <Checkbox
        onChange={() => {
          setInformUser(!informUser);
        }}
        isChecked={informUser}
        isDisabled={!user.emailConfirmed}
      >
        <Text fontSize="sm">Update user about their account deletion? </Text>
      </Checkbox>
    </VStack>
  );
};
