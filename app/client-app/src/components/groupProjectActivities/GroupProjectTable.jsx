import { HStack, Button, Text, VStack } from "@chakra-ui/react";
import { FaPlus } from "react-icons/fa";
import { DataTable } from "components/dataTable/DataTable";
import { useEffect, useMemo, useState } from "react";
import { useFormikContext, Formik } from "formik";
import { groupProjectTableColumn } from "./groupProjectTableColumn";

export const GroupProjectTable = ({ name, label, isDisabled }) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);
  const columns = useMemo(
    () => groupProjectTableColumn({ isDisabled }),
    [isDisabled]
  ); // array of objects to be used for the DataTable columns

  return (
    <VStack align="flex-start">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={columns}
        tableLabel={label}
        FooterCellAddRow={
          !isDisabled && <FooterCell handleAddRow={handleAddRow} />
        }
      >
        <HStack flex={1}>
          <Text size="sm" as="b">
            Group Summary Table
          </Text>
        </HStack>
      </DataTable>
    </VStack>
  );
};

const handleAddRow = () => {
  const newRow = columns
    .filter((column) => "accessorKey" in column) // filter out columns without property 'accessorKey'
    .reduce((acc, column) => {
      return {
        ...acc,
        [column.accessorKey]:
          column.accessorKey === "serialNumber"
            ? tableData.length + 1
            : column.accessorKey === "weekDate"
            ? ""
            : false,
      };
    }, {});

  setTableData((old) => [...old, newRow]);
};

export const FooterCell = ({ handleAddRow }) => {
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
