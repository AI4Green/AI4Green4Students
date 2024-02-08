import { HStack, Button, Text, VStack } from "@chakra-ui/react";
import { FaPlus } from "react-icons/fa";
import { DataTable } from "components/dataTable/DataTable";
import { useEffect, useMemo, useState } from "react";
import { useFormikContext, Formik } from "formik";
import { TableCellTextInput } from "components/dataTable/DataTableCellItems";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";

export const HazardSummaryTable = ({ name, label, isDisabled }) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);

  return (
    <CDTable
      tableData={tableData}
      setTableData={setTableData}
      tableLabel={label}
      isDisabled={isDisabled}
    />
  );
};

const CDTable = ({ tableData, setTableData, tableLabel, isDisabled }) => {
  const columns = [
    {
      accessorKey: "serialNumber",
      header: () => <DataTableColumnHeader sorting={false} title="No." />,
      enableHiding: false,
    },
    {
      accessorKey: "chemicals",
      header: () => <DataTableColumnHeader sorting={false} title="Chemicals" />,
      cell: ({ getValue, row, column, table }) => (
        <TableCellTextInput
          {...{ getValue, row, column, table }}
          isDisabled={isDisabled}
          placeholder="chemicals"
        />
      ),
    },
    {
      accessorKey: "hazard",
      header: () => <DataTableColumnHeader sorting={false} title="Hazards" />,
      cell: ({ getValue, row, column, table }) => (
        <TableCellTextInput
          {...{ getValue, row, column, table }}
          isDisabled={isDisabled}
        />
      ),
    },
    {
      accessorKey: "precautions",
      header: () => (
        <DataTableColumnHeader sorting={false} title="Precautions" />
      ),
      cell: ({ getValue, row, column, table }) => (
        <TableCellTextInput
          {...{ getValue, row, column, table }}
          isDisabled={isDisabled}
        />
      ),
    },
  ];

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

  return (
    <VStack align="flex-start">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={columns}
        FooterCellAddRow={
          !isDisabled && <FooterCell handleAddRow={handleAddRow} />
        }
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
