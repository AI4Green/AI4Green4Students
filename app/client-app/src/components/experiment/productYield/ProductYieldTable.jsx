import { Button, HStack, Text, VStack } from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { useFormikContext } from "formik";
import { useEffect, useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";
import { productYieldTableColumn } from "./productYieldTableColumn";

export const ProductYieldTable = ({ name, label, isDisabled }) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);

  const columns = useMemo(
    () => productYieldTableColumn(isDisabled),
    [isDisabled]
  );

  const handleAddRow = () => {
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

  return (
    <VStack align="flex-start" w="full">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={columns}
        FooterCellAddRow={
          !isDisabled && (
            <AddRowButton
              handleAddRow={() =>
                handleAddRow(columns, setTableData, tableData)
              }
            />
          )
        }
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

const AddRowButton = ({ handleAddRow }) => {
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
