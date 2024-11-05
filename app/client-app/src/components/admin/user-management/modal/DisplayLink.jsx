import {
  Alert,
  AlertIcon,
  Box,
  Code,
  HStack,
  IconButton,
  VStack,
  useToast,
} from "@chakra-ui/react";
import { FaRegCopy } from "react-icons/fa";

export const DisplayLink = ({
  displayLink, // could be activation link, password reset link
  linkType,
  children,
}) => {
  const toast = useToast();

  const onClickCopyToClipboard = (value) => {
    // handle copy to clipboard action
    navigator.clipboard.writeText(value);
    toast({
      position: "top",
      title: "Copied to clipboard",
      status: "success",
      duration: 700,
      isClosable: true,
    });
  };
  return (
    <Box borderWidth={1} borderRadius={7}>
      <VStack align="flex-start">
        <Alert status="info">
          <AlertIcon />
          Please copy the {linkType} link and pass it to the user to complete
          the {linkType} process.
        </Alert>
      </VStack>
      <HStack p={2}>
        <Code flex={1} py={1} px={2} borderRadius={8}>
          {displayLink}
        </Code>
        ;
        <IconButton
          variant="solid"
          onClick={() => onClickCopyToClipboard(displayLink)}
          size="sm"
          icon={<FaRegCopy />}
        />
      </HStack>
      {children}
    </Box>
  );
};
