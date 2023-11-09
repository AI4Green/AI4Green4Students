import {
  Button,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  MenuDivider,
  Text,
  Box,
} from "@chakra-ui/react";

import { FaChevronDown, FaChevronUp, FaSort, FaEyeSlash } from "react-icons/fa";

export const DataTableColumnHeader = ({ column, title }) => {
  if (!column.getCanSort()) {
    return <Text>{title}</Text>;
  }

  return (
    <Box>
      <Menu>
        <MenuButton
          as={Button}
          variant="ghost"
          size="sm"
          px={0}
          rightIcon={
            column.getIsSorted() === "desc" ? (
              <FaChevronDown />
            ) : column.getIsSorted() === "asc" ? (
              <FaChevronUp />
            ) : (
              <FaSort />
            )
          }
        >
          {title}
        </MenuButton>
        <MenuList>
          <MenuItem
            onClick={() => column.toggleSorting(false)}
            icon={<FaChevronUp />}
          >
            Asc
          </MenuItem>
          <MenuItem
            onClick={() => column.toggleSorting(true)}
            icon={<FaChevronDown />}
          >
            Desc
          </MenuItem>
          <MenuDivider />

          <MenuItem
            onClick={() => column.toggleVisibility(false)}
            icon={<FaEyeSlash />}
          >
            Hide
          </MenuItem>
        </MenuList>
      </Menu>
    </Box>
  );
};
