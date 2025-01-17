import {
  Box,
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
import { FieldImage } from "./field-image";

export const ReactionSchemeTable = ({ fieldResponse, sectionId, recordId }) => {
  const reactionTable = fieldResponse?.reactionTable;
  const reactionSchemeImage = fieldResponse.reactionSketch?.reactionImage;

  return (
    <Box maxW="full">
      {reactionSchemeImage && (
        <FieldImage
          sectionId={sectionId}
          recordId={recordId}
          image={reactionSchemeImage}
        />
      )}

      <Text fontWeight="normal" fontSize="sm" mb={2}>
        Reaction Table
      </Text>
      <ReactionTable reactionTable={reactionTable} />
    </Box>
  );
};

const ReactionTable = ({ reactionTable }) => {
  const columns = [
    "Substances Used",
    "Type",
    "Limiting",
    "Mass (Vol)",
    "g/l/s (Physical form)",
    "Mol.Wt",
    "Amount (mmol)",
    "Density",
    "Hazards",
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
          {reactionTable &&
            reactionTable.map((reaction, i) => (
              <Tr key={i}>
                <Td>{reaction.substancesUsed}</Td>
                <Td>{reaction.substanceType}</Td>
                <Td>
                  <Checkbox isChecked={reaction?.limiting} isDisabled />
                </Td>
                <Td fontSize={!reaction.mass?.value && "xxs"}>
                  {reaction.mass.value || NO_RESPONSE} {reaction.mass.unit}
                </Td>
                <Td fontSize={!reaction.gls && "xxs"}>
                  {reaction.gls || NO_RESPONSE}
                </Td>
                <Td>{reaction.molWeight}</Td>
                <Td fontSize={!reaction.amount && "xxs"}>
                  {reaction.amount || NO_RESPONSE}
                </Td>
                <Td>{reaction.density}</Td>
                <Td fontSize={!reaction.hazardsInput && "xxs"}>
                  <VStack>
                    <Text>
                      {reaction.hazardsInput?.hazardCodes || NO_RESPONSE}
                    </Text>
                    <Text borderWidth={1} px={2} w="full" borderRadius={4}>
                      {reaction.hazardsInput?.hazardDescription || NO_RESPONSE}
                    </Text>
                  </VStack>
                </Td>
              </Tr>
            ))}
        </Tbody>
      </Table>
    </TableContainer>
  );
};
