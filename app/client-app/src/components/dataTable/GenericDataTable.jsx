import { Button, HStack, Text, VStack } from "@chakra-ui/react";
import { useFormikContext } from "formik";
import { useEffect, useState } from "react";
import { DataTable } from "./DataTable";
import { FaPlus } from "react-icons/fa";

/**
 * Formik ready data table component which have been derived from DataTable component.
 * Useful for creating tables quickly for formik.
 * @param {string} name - Name of the formik field.
 * @param {string} label - Label for the table.
 * @param {array} columns - Columns for the table.
 * @param {React.Component} FooterCellAddRow - Footer cell component for adding new row.
 */

export const GenericDataTable = ({
  name,
  label,
  columns,
  FooterCellAddRow,
}) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);

  return (
    <VStack align="flex-start" w="full">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={columns}
        FooterCellAddRow={FooterCellAddRow}
      >
        <HStack flex={1}>
          <Text size="sm" as="b">
            {label}
          </Text>
        </HStack>
      </DataTable>
    </VStack>
  );
};

/**
 * Generic function that can be used for adding new row to the table.
 * @param {*} columns - Columns of the table.
 * @param {*} tableData - Table data.
 * @param {*} setTableData - Setter for table data.
 */
export const handleGenericAddRow = (columns, tableData, setTableData) => {
  const newRow = columns
    .filter((column) => "accessorKey" in column)
    .reduce((acc, column) => {
      return {
        ...acc,
        [column.accessorKey]:
          column.accessorKey === "serialNumber" ? tableData.length + 1 : "",
      };
    }, {});

  setTableData((old) => [...old, newRow]);
};

/**
 * Generic footer cell component for adding new row.
 * @param {*} handleAddRow - Function to add new row.
 */
export const GenericFooterCell = ({ handleAddRow }) => {
  return (
    <Button
      colorScheme="blue"
      size="xs"
      leftIcon={<FaPlus />}
      onClick={handleAddRow}
    >
      Add new
    </Button>
  );
};
