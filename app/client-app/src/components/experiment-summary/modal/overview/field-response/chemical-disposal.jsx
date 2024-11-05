import {
  Checkbox,
  Table,
  TableContainer,
  Tbody,
  Td,
  Text,
  Th,
  Thead,
  Tr,
  VStack,
} from "@chakra-ui/react";
import { NO_RESPONSE } from ".";

export const ChemicalDisposalTable = ({ fieldResponse }) => {
  const columns = [
    "No.",
    "Chemical",
    "Halogenated Solvent",
    "Non-Halogenated Solvent",
    "Waste Store",
    "Aqueous (Non Hazardous)",
    "Other (specify)",
  ];

  return (
    <TableContainer borderRadius={7} borderWidth={1}>
      <Table variant="simple" colorScheme="gray" fontSize="xs" borderRadius={7}>
        <Thead bgColor="gray.50">
          <Tr>
            {columns.map((column, i) => (
              <Th
                key={i}
                textTransform="none"
                whiteSpace="normal"
                w="10px"
                fontWeight="semibold"
              >
                {column}
              </Th>
            ))}
          </Tr>
        </Thead>
        <Tbody color="gray.500">
          {fieldResponse?.map((item) => (
            <Tr key={item.serialNumber}>
              <Td>{item.serialNumber || "N/A"}</Td>
              <Td>{item.chemical || "N/A"}</Td>
              <Td>
                <Checkbox isChecked={item.halogenatedSolvent} isDisabled />
              </Td>
              <Td>
                <Checkbox isChecked={item.nonHalogenatedSolvent} isDisabled />
              </Td>
              <Td>
                <Checkbox isChecked={item.wasteStore} isDisabled />
              </Td>
              <Td>
                <Checkbox isChecked={item.aqueousNonHazardous} isDisabled />
              </Td>
              <Td>
                <VStack align="start">
                  <Checkbox isChecked={item.others.status} isDisabled />
                  {item.others.status && (
                    <Text
                      borderWidth={1}
                      px={2}
                      w="full"
                      borderRadius={4}
                      fontSize="xxs"
                    >
                      {item.others.value || NO_RESPONSE}
                    </Text>
                  )}
                </VStack>
              </Td>
            </Tr>
          ))}
        </Tbody>
      </Table>
    </TableContainer>
  );
};
