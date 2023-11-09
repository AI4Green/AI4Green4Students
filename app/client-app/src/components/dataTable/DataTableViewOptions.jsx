import { FaChevronDown, FaCheck, FaEyeSlash } from "react-icons/fa";
import {
  Button,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  Text,
  MenuGroup,
  Box,
} from "@chakra-ui/react";

export function DataTableViewOptions({ table }) {
  return (
    <Box>
      <Menu>
        <MenuButton
          as={Button}
          size="sm"
          variant="outline"
          rightIcon={<FaChevronDown />}
        >
          <Text fontSize="sm"> View</Text>
        </MenuButton>
        <MenuList align="end" minWidth="150px">
          <MenuGroup title="Table view options">
            {table
              .getAllColumns()
              .filter(
                (column) =>
                  typeof column.accessorFn !== "undefined" &&
                  column.getCanHide()
              )
              .map((column) => {
                return (
                  <MenuItem
                    icon={column.getIsVisible() ? <FaCheck /> : <FaEyeSlash />}
                    key={column.id}
                    textTransform="capitalize"
                    onClick={() =>
                      column.toggleVisibility(!column.getIsVisible())
                    }
                  >
                    <Text fontSize="sm">{column.id}</Text>
                  </MenuItem>
                );
              })}
          </MenuGroup>
        </MenuList>
      </Menu>
    </Box>
  );
}
