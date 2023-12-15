import {
  Button,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Box,
} from "@chakra-ui/react";
import { FaChevronDown } from "react-icons/fa";

export const ActionButton = ({ label = "Actions", actions, ...p }) => {
  // get only eligible actions, by their own criteria
  const eligibleActions = Object.values(actions).filter((action) =>
    action.isEligible()
  );

  switch (eligibleActions.length) {
    case 0:
      return null;
    case 1:
      return (
        <Button
          leftIcon={eligibleActions[0].icon}
          colorScheme={eligibleActions[0].colorScheme}
          size="sm"
          variant="solid"
          onClick={eligibleActions[0].onClick}
          {...p}
        >
          {eligibleActions[0].label}
        </Button>
      );
    default:
      return (
        <Box>
          <Menu>
            <MenuButton
              as={Button}
              rightIcon={<FaChevronDown />}
              colorScheme="blue"
              size="sm"
              {...p}
            >
              {label}
            </MenuButton>
            <MenuList>
              {eligibleActions.map((x, index) => (
                <MenuItem icon={x.icon} onClick={x.onClick} key={index}>
                  {x.label}
                </MenuItem>
              ))}
            </MenuList>
          </Menu>
        </Box>
      );
  }
};
