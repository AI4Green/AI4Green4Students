/*
This component is renderd when user saves their reaction sketch.
User can input a reaction description and a relevant inputs in a table to generate a summary.
*/

import { Heading, VStack } from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { reactionTableColumns } from "./reactionTableColumn";
import { ActionButton } from "components/ActionButton";
import { GiMaterialsScience } from "react-icons/gi";
import { useEffect, useState } from "react";
import { useFormikContext } from "formik";

export const ReactionTable = ({ name }) => {
  const { values, setFieldValue } = useFormikContext();
  const [tableData, setTableData] = useState(values[name]);

  useEffect(() => setFieldValue(name, tableData), [tableData]);

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
        <Heading size="sm" as="u">
          Please fill in the relevant fields below
        </Heading>

        <RTableActions />
      </DataTable>
    </VStack>
  );
};

const RTableActions = ({ row }) => {
  const experimentActions = {
    addRow: {
      // TODO: Trigger a modal to add a new row to the table
      isEligible: () => true,
      icon: <GiMaterialsScience />,
      label: "Add Row",
    },
    generateSummary: {
      // Capture current table data, update/save field value and generate summary
      isEligible: () => true,
      icon: <GiMaterialsScience />,
      label: "Generate Summary",
    },
  };
  return (
    <ActionButton
      label="Table actions"
      actions={experimentActions}
      size="sm"
      colorScheme="gray"
      variant="outline"
    />
  );
};
