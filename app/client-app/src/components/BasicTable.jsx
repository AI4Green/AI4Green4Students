import {
  Box,
  TableContainer,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
} from "@chakra-ui/react";
import { useTable, useExpanded } from "react-table";

export const BasicTable = ({ columns, data }) => {
  const {
    headerGroups,
    rows,
    prepareRow,
    state: { expanded },
  } = useTable(
    {
      columns,
      data,
    },
    useExpanded
  );

  const TableHeader = () => (
    <Thead bgColor="gray.50">
      {headerGroups.map((headerGroup) => (
        <Tr key={headerGroup.getHeaderGroupProps().key}>
          {headerGroup.headers.map((column) => (
            <Th key={column.getHeaderProps().key} py={3}>
              {column.render("Header")}
            </Th>
          ))}
        </Tr>
      ))}
    </Thead>
  );

  const TableData = () =>
    rows.map((row) => {
      prepareRow(row);
      return (
        <Tr
          key={row.getRowProps().key}
          bgColor={row.isExpanded ? "blackAlpha.100" : ""}
          _hover={{
            bg: "blackAlpha.50",
          }}
        >
          {row.cells.map((cell) => (
            <Td key={cell.getCellProps().key} py={row.depth === 0 ? 4 : 2}>
              {cell.render("Cell")}
            </Td>
          ))}
        </Tr>
      );
    });

  return (
    <Box p="6" w="100%">
      <TableContainer borderRadius={7} borderWidth={1}>
        <Table variant="simple" colorScheme="blue" size="sm" borderRadius={5}>
          <TableHeader />
          <Tbody>
            <TableData />
          </Tbody>
        </Table>
      </TableContainer>
    </Box>
  );
};
