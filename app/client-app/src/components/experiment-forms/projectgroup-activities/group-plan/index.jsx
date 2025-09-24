import { Button, FormLabel, HStack, VStack } from "@chakra-ui/react";
import { useProjectGroup } from "api";
import { DataTable } from "components/core/data-table";
import { useUser } from "contexts";
import { useFormikContext } from "formik";
import { useEffect, useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";
import { useParams } from "react-router-dom";

import { groupPlanTableColumn } from "./columns";

export const GroupPlanTable = ({ name, label, isDisabled }) => {
  const { projectGroupId } = useParams();

  const { data: projectGroup } = useProjectGroup(projectGroupId);

  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name] || []);

  useEffect(() => {
    if (tableData !== values[name]) {
      setFieldValue(name, tableData);
    }
  }, [tableData, name, setFieldValue, values]);

  const { user } = useUser();

  const columns = useMemo(
    () => groupPlanTableColumn(projectGroup?.students, user, isDisabled),
    [projectGroup?.students, user, isDisabled]
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
      size="sm"
      leftIcon={<FaPlus />}
      onClick={handleAddRow}
      variant="ghost"
    >
      Add new
    </Button>
  );
};
