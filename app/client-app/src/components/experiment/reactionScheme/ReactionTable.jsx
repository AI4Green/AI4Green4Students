/*
This component is renderd when user saves their reaction sketch.
User can input a reaction description and a relevant inputs in a table to generate a summary.
*/

import { HStack, Text, VStack } from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { reactionTableColumns } from "./reactionTableColumn";
import { useEffect, useState } from "react";
import { useField } from "formik";
import { useInitialReactionTableData } from "./useReactionTableData";

export const ReactionTable = ({ name, ketcherData }) => {
  const [field, meta, helpers] = useField(name);

  const { initialTableData } = useInitialReactionTableData(ketcherData);

  const [tableData, setTableData] = useState(field.value || initialTableData);

  useEffect(() => helpers.setValue(tableData), [tableData]);

  return <RTable tableData={tableData} setTableData={setTableData} />;
};

const RTable = ({ tableData, setTableData }) => {
  return (
    <VStack align="flex-start">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={reactionTableColumns()}
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
