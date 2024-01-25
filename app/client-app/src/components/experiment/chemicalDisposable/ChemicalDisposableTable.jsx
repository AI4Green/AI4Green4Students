import { HStack, Button, Text, VStack } from "@chakra-ui/react";
import { FaPlus } from "react-icons/fa";
import { DataTable } from "components/dataTable/DataTable";
import { chemicalDisposableTableColumns } from "./chemicalDisposableTableColumn";
import { useEffect, useState } from "react";
import { useFormikContext } from "formik";

export const ChemicalDisposableTable = ({ name, label }) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);

  return (
    <CDTable
      tableData={tableData}
      setTableData={setTableData}
      tableLabel={label}
    />
  );
};

const CDTable = ({ tableData, setTableData, tableLabel }) => {
  const columns = chemicalDisposableTableColumns();

  const handleAddRow = () => {
    const newRow = columns
      .filter((column) => "accessorKey" in column) // filter out columns without property 'accessorKey'
      .reduce((acc, column) => {
        return {
          ...acc,
          [column.accessorKey]:
            column.accessorKey === "serialNumber"
              ? tableData.length + 1
              : column.accessorKey === "chemical"
              ? ""
              : false,
        };
      }, {});

    setTableData((old) => [...old, newRow]);
  };

  return (
    <VStack align="flex-start">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={columns}
        FooterCellAddRow={<FooterCell handleAddRow={handleAddRow} />}
      >
        <HStack flex={1}>
          <Text size="sm" as="b">
            {tableLabel}
          </Text>
        </HStack>
      </DataTable>
    </VStack>
  );
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
