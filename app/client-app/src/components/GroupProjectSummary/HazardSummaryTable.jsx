import React, { useState } from "react";
import {
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Input,
  Button,
} from "@chakra-ui/react";

export const HazardSummaryTable = () => {
  const [tableData, setTableData] = useState([
    { id: 1, chemicals: "", hazard: "", precautions: "" },
  ]);

  const handleInputChange = (id, column, value) => {
    setTableData((prevData) =>
      prevData.map((row) => (row.id === id ? { ...row, [column]: value } : row))
    );
  };
  const handleSubmit = () => {
    // Handle the table data submission, e.g., display it
    console.log("Table Data Submitted:", tableData);
  };

  const handleAddRow = () => {
    setTableData((prevData) => [
      ...prevData,
      { id: prevData.length + 1, chemicals: "", hazard: "", precautions: "" },
    ]);
  };

  return (
    <Table variant="striped" colorScheme="teal">
      <Thead>
        <Tr>
          <Th>Chemicals</Th>
          <Th>Hazards</Th>
          <Th>Precaution</Th>
        </Tr>
      </Thead>
      <Tbody>
        {tableData.map((row) => (
          <Tr key={row.id}>
            <Td>
              <Input
                value={row.column1}
                onChange={(e) =>
                  handleInputChange(row.id, "chemicals", e.target.value)
                }
              />
            </Td>
            <Td>
              <Input
                value={row.column2}
                onChange={(e) =>
                  handleInputChange(row.id, "hazard", e.target.value)
                }
              />
            </Td>
            <Td>
              <Input
                value={row.column3}
                onChange={(e) =>
                  handleInputChange(row.id, "precautions", e.target.value)
                }
              />
            </Td>
          </Tr>
        ))}
      </Tbody>
      <Button colorScheme="teal" onClick={handleAddRow}>
        Add Row
      </Button>

      <Button colorScheme="teal" onClick={handleSubmit}>
        Submit
      </Button>
    </Table>
  );
};
