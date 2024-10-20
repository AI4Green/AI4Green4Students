import { HStack, Button, FormLabel, VStack } from "@chakra-ui/react";
import { FaPlus } from "react-icons/fa";
import { DataTable } from "components/core/data-table/table";
import { useEffect, useMemo, useState } from "react";
import { useFormikContext } from "formik";
import { groupPlanTableColumn } from "./groupPlanTableColumn";
import { useSectionForm } from "contexts/SectionForm";
import { useUser } from "contexts/User";

export const GroupPlanTable = ({ name, label, isDisabled }) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);

  const {
    projectGroup: { students },
  } = useSectionForm();
  const { user } = useUser();

  const columns = useMemo(
    () => groupPlanTableColumn(students, user, isDisabled),
    [students, isDisabled]
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
          !isDisabled && <FooterCell handleAddRow={handleAddRow} />
        }
      >
        <HStack flex={1}>
          <FormLabel>{label}</FormLabel>
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
