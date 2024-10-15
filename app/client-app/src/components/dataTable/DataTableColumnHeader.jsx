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

export const DataTableColumnHeader = ({ sorting, column, title, w = 0 }) => {
  if (!sorting && !column?.getCanSort()) {
    return (
      <Text fontSize="sm" fontWeight="semibold" noOfLines={3}>
        {title}
      </Text>
    );
  }

  return (
    <Box align="start" w={w}>
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
          _hover={{ bg: "transparent" }}
          _active={{ bg: "transparent" }}
        >
          <Text fontSize="sm" fontWeight="semibold" noOfLines={3}>
            {title}
          </Text>
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
