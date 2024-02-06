import React, { useState } from "react";
import {
  ChakraProvider,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Textarea,
  Box,
} from "@chakra-ui/react";

export const LiteratureSummaryTable = () => {
  const [text, setText] = useState("");

  const handleTextareaChange = (e) => {
    const newText = e.target.value;
    if (countWords(newText) <= 200) {
      setText(newText);
    }
  };

  const countWords = (text) => {
    return text.trim().split(/\s+/).length;
  };

  return (
    <Table variant="striped" colorScheme="teal">
      <Thead>
        <Tr>
          <Th>Literature Summary (Max 200 Words)</Th>
        </Tr>
      </Thead>
      <Tbody>
        <Tr>
          <Td>
            <Textarea
              value={text}
              onChange={handleTextareaChange}
              placeholder="Enter text here..."
            />
            <Box mt={2}>Word Count: {countWords(text)} / 200</Box>
          </Td>
        </Tr>
      </Tbody>
    </Table>
  );
};
