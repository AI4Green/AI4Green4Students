import {
  Icon,
  IconButton,
  Popover,
  PopoverArrow,
  PopoverBody,
  PopoverCloseButton,
  PopoverContent,
  PopoverTrigger,
} from "@chakra-ui/react";
import { FaCheckCircle, FaExclamationCircle } from "react-icons/fa";

//TODO: add validation for the hazards against the backend
export const HazardsValidation = ({ input, valid }) => {
  if (!input || !valid) return null;

  return input === valid ? (
    <Icon as={FaCheckCircle} color="green.500" fontSize="lg" />
  ) : (
    <Popover>
      <PopoverTrigger>
        <IconButton
          aria-label="warning"
          icon={
            <Icon as={FaExclamationCircle} color="orange.500" fontSize="lg" />
          }
          size="sm"
          variant="ghost"
        />
      </PopoverTrigger>
      <PopoverContent color="white" bg="orange.500">
        <PopoverArrow />
        <PopoverCloseButton />
        <PopoverBody fontWeight="bold" border="0">
          Incorrect hazard code
        </PopoverBody>
      </PopoverContent>
    </Popover>
  );
};
