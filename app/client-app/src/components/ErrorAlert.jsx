import { Alert, AlertDescription, AlertIcon } from "@chakra-ui/react";

export const ErrorAlert = ({ status, message }) => {
  return (
    <Alert status={status}>
      <AlertIcon />
      <AlertDescription>{message}</AlertDescription>
    </Alert>
  );
};
