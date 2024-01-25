import { useState } from "react";
import {
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  getFilteredRowModel,
  getExpandedRowModel,
  useReactTable,
} from "@tanstack/react-table";
import {
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  TableContainer,
  Box,
  HStack,
  Tfoot,
} from "@chakra-ui/react";
import { DataTableViewOptions } from "./DataTableViewOptions";

export function DataTable({
  columns,
  data,
  setTableData,
  globalFilter,
  FooterCellAddRow, // TODO: In future, we can add make this flexible.
  children,
}) {
  const [sorting, setSorting] = useState([]);
  const [expanded, setExpanded] = useState({});

  const table = useReactTable({
    data,
    columns,
    state: {
      sorting,
      expanded,
      globalFilter,
    },
    meta: {
      updateData: (rowIndex, columnId, value) =>
        setTableData((prev) =>
          prev.map((row, index) =>
            index === rowIndex
              ? {
                  ...prev[rowIndex],
                  [columnId]: value,
                }
              : row
          )
        ),
      removeRow: (rowIndex) => {
        setTableData((prev) =>
          prev
            .filter((_row, index) => index !== rowIndex)
            .map((row, index) => ({ ...row, serialNumber: index + 1 }))
        );
      },
    },
    getSubRows: (row) => row.subRows,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getExpandedRowModel: getExpandedRowModel(),
    onSortingChange: setSorting,
    onExpandedChange: setExpanded,
    filterFromLeafRows: true,
  });

  return (
    <Box p="6" w="100%">
      <HStack justify="flex-end" py={2} mb={2} spacing={5}>
        {children}
        <DataTableViewOptions table={table} />
      </HStack>
      <TableContainer borderRadius={7} borderWidth={1}>
        <Table variant="simple" colorScheme="blue" size="sm" borderRadius={5}>
          <Thead bgColor="gray.50">
            {table.getHeaderGroups().map((headerGroup) => (
              <Tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <Th key={header.id} textTransform="none" whiteSpace="normal">
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </Th>
                ))}
              </Tr>
            ))}
          </Thead>
          <Tbody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <Tr
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                  bgColor={row.getIsExpanded() ? "blackAlpha.100" : ""}
                  _hover={{
                    bg: "blackAlpha.50",
                  }}
                >
                  {row.getVisibleCells().map((cell) => (
                    <Td key={cell.id}>
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext()
                      )}
                    </Td>
                  ))}
                </Tr>
              ))
            ) : (
              <Tr>
                <Td colSpan={columns.length} textAlign="center">
                  No data available
                </Td>
              </Tr>
            )}
          </Tbody>
          {FooterCellAddRow && (
            <Tfoot>
              <Tr>
                <Th colSpan={columns.length} textAlign="right">
                  {FooterCellAddRow}
                </Th>
              </Tr>
            </Tfoot>
          )}
        </Table>
      </TableContainer>
    </Box>
  );
}
