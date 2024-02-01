/**
 * The component renders a table.
 * Populates table using ketcher data if there's no existing table data.
 * Data is reset if ketcher user resets the ketcher.
 */

import { HStack, Select, Text, VStack } from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { reactionTableColumns } from "./reactionTableColumn";
import { useEffect, useState } from "react";
import { useField } from "formik";
import { useInitialReactionTableData } from "./useReactionTableData";

export const REACTION_TABLE_DEFAULT_VALUES = {
  tableData: [],
  massUnit: "",
};

export const ReactionTable = ({ name, ketcherData, isDisabled }) => {
  const [field, meta, helpers] = useField(name);

  const { initialTableData } = useInitialReactionTableData(ketcherData);

  const [tableData, setTableData] = useState(
    field.value?.tableData || initialTableData
  );
  const [massUnit, setMassUnit] = useState(field.value?.massUnit || "cm3");

  const tableColumns = reactionTableColumns({
    isDisabled,
    massColumnHeaderDropdown: {
      ColumnUnitHeaderDropdown,
      props: {
        options: [
          { value: "cm3", label: "cm3" },
          { value: "g", label: "g" },
        ],
        value: massUnit,
        setValue: setMassUnit,
      },
    },
  }); // array of objects to be used for the DataTable columns

  useEffect(() => {
    helpers.setValue({ ...field.value, tableData });
  }, [tableData]);

  useEffect(() => {
    helpers.setValue({ ...field.value, massUnit });
  }, [massUnit]);

  return (
    <VStack align="flex-start">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={tableColumns}
      >
        <HStack flex={1}>
          <Text size="sm" as="b">
            Please fill in the relevant fields below
          </Text>
        </HStack>
      </DataTable>
    </VStack>
  );
};

const ColumnUnitHeaderDropdown = ({ options, value, setValue, isDisabled }) => (
  <Select
    minW="80px"
    size="xs"
    value={value}
    onChange={(e) => setValue(e.target.value)}
    isDisabled={isDisabled}
    placeholder="Select"
  >
    {options.map((option, index) => (
      <option key={index} value={option.value}>
        {option.label}
      </option>
    ))}
  </Select>
);
