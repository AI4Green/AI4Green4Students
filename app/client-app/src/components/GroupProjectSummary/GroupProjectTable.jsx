import React, { useState, useMemo } from "react";
import {
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Accordion,
  AccordionItem,
  AccordionButton,
  AccordionPanel,
  AccordionIcon,
  Input,
} from "@chakra-ui/react";

import { DataTable } from "components/dataTable/DataTable";
import { groupProjectTableColumn } from "./GroupProjectTableColumn";
import { projectManagementColumns } from "components/projectManagement/table/projectManagementColumns";

export const GroupProjectTable = () => {
  const columns = useMemo(() => groupProjectTableColumn, []);

  const [data, setData] = useState([]);

  // useEffect(() => setValue(cell, data), [data]);

  // const { getTableProps, getTableBodyProps, headerGroups, rows, prepareRow } =
  //   useTable({
  //     columns,
  //     data,
  //   });

  return <DataTable data={data} columns={columns} setTableData={setData} />;

  {
    /* <Table {...getTableProps()} variant="striped" colorScheme="teal">
      <Thead>
        {headerGroups.map((headerGroup) => (
          <Tr {...headerGroup.getHeaderGroupProps()}>
            {headerGroup.headers.map((column) => (
              <Th {...column.getHeaderProps()}>{column.render("Header")}</Th>
            ))}
          </Tr>
        ))}
      </Thead>
      <Tbody {...getTableBodyProps()}>
        {rows.map((row) => {
          prepareRow(row);
          return (
            <React.Fragment key={row.id}>
              <Tr {...row.getRowProps()}>
                {row.cells.map((cell) => (
                  <Td {...cell.getCellProps()}>{cell.render("Cell")}</Td>
                ))}
              </Tr>
              <Accordion allowToggle>
                <AccordionItem>
                  <AccordionButton>
                    <AccordionIcon />
                    Details
                  </AccordionButton>
                  <AccordionPanel>
                    {columns.map((column, index) => (
                      <div key={index}>
                        <span>{column.Header}:</span>
                        <Input
                          value={row.values[column.accessor]}
                          onChange={(e) =>
                            (row.original[column.accessor] = e.target.value)
                          }
                        />
                      </div>
                    ))}
                  </AccordionPanel>
                </AccordionItem>
              </Accordion>
            </React.Fragment>
          );
        })}
      </Tbody>
    </Table>
  ); */
  }
};
