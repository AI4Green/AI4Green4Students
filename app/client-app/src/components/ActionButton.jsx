import {
  Button,
  IconButton,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Box,
  Portal,
  Text,
} from "@chakra-ui/react";
import { FaChevronDown, FaEllipsisH, FaBattleNet } from "react-icons/fa";
import { useMemo } from "react";

export const ActionButton = ({
  label,
  LeftIcon = FaBattleNet,
  actions,
  ...p
}) => {
  const eligibleActions = useMemo(
    () => Object.values(actions).filter((action) => action.isEligible()),
    [actions]
  );

  return (
    <Box>
      <Menu>
        <MenuButton
          as={label ? Button : IconButton}
          leftIcon={label && <LeftIcon />}
          rightIcon={label && <FaChevronDown />}
          aria-label={label ? label : "Actions"}
          icon={<FaEllipsisH />}
          variant={label ? "outline" : "ghost"}
          colorScheme="gray"
          size="sm"
          {...p}
        >
          {label}
        </MenuButton>
        <Portal>
          <MenuList>
            {eligibleActions.length === 0 ? (
              <MenuItem isDisabled>No actions available.</MenuItem>
            ) : (
              eligibleActions.map((x, index) => (
                <MenuItem icon={x.icon} onClick={x.onClick} key={index}>
                  <Text fontSize="sm">{x.label}</Text>
                </MenuItem>
              ))
            )}
          </MenuList>
        </Portal>
      </Menu>
    </Box>
  );
};
