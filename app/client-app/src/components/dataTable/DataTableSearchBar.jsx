import { HStack, InputGroup, InputLeftElement, Input } from "@chakra-ui/react";
import { FaSearch } from "react-icons/fa";

export const DataTableSearchBar = ({
  setSearchValue,
  searchValue,
  placeholder,
}) => (
  <HStack>
    <InputGroup>
      <InputLeftElement pointerEvents="none" height="100%">
        <FaSearch color="gray" />
      </InputLeftElement>
      <Input
        variant="outline"
        borderColor="gray.400"
        size="sm"
        borderRadius={6}
        placeholder={placeholder}
        _placeholder={{ opacity: 1 }}
        onChange={(e) => setSearchValue(e.target.value)}
        value={searchValue || ""}
      />
    </InputGroup>
  </HStack>
);
