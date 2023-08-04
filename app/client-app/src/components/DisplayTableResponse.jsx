import {
  Table,
  TableContainer,
  Tbody,
  Td,
  Th,
  Thead,
  Tr,
} from "@chakra-ui/react";
import { Question } from "./forms/Question";

export const DisplayTableResponse = ({
  questionNumber,
  question,
  columns,
  children,
}) => {
  return (
    <>
      <Question number={questionNumber} pt={2}>
        {question}
      </Question>

      {columns && (
        <TableContainer pt={2}>
          <Table variant="simple">
            <Thead>
              <Tr>
                {columns.map((x, i) => (
                  <Th key={i}>{x}</Th>
                ))}
              </Tr>
            </Thead>
            <Tbody>
              {children &&
                children.map((data, i) => (
                  <Tr key={i}>
                    {
                      // iterate on `columns` as that defines the number of valid columns
                      // but use the value from the current `data` record's matching index
                      columns.map((_, i) => (
                        <Td key={i}>{data[i]}</Td>
                      ))
                    }
                  </Tr>
                ))}
            </Tbody>
          </Table>
        </TableContainer>
      )}
    </>
  );
};
