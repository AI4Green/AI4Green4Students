/**
 * The component renders a table.
 * Populates table using ketcher data if there's no existing table data.
 * Data is reset if ketcher user resets the ketcher.
 */

import {
  Button,
  HStack,
  Icon,
  IconButton,
  Popover,
  PopoverArrow,
  PopoverBody,
  PopoverCloseButton,
  PopoverContent,
  PopoverTrigger,
  Select,
  Text,
  VStack,
  useDisclosure,
} from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { reactionTableColumns } from "./reactionTableColumn";
import { useEffect, useState } from "react";
import { useField } from "formik";
import { useInitialReactionTableData } from "./useReactionTableData";
import { FaCheckCircle, FaExclamationCircle, FaPlus } from "react-icons/fa";
import { AddSubstanceModal } from "./AddSubstanceModal";

export const REACTION_TABLE_DEFAULT_VALUES = {
  tableData: [],
  massUnit: "",
};

export const ReactionTable = ({ name, ketcherData, isDisabled }) => {
  const [field, meta, helpers] = useField(name);
  const hasExistingTableData = field.value?.tableData?.length >= 1;

  const { initialTableData } = useInitialReactionTableData(ketcherData);
  const initialData = hasExistingTableData
    ? field.value.tableData
    : initialTableData;

  const [tableData, setTableData] = useState(initialData);

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
        FooterCellAddRow={<FooterCell {...{ setTableData }} />}
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

export const FooterCell = ({ tableColumns, setTableData }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <>
      <Button
        colorScheme="blue"
        size="xs"
        leftIcon={<FaPlus />}
        onClick={onOpen}
      >
        Add new substance
      </Button>
      <AddSubstanceModal
        isModalOpen={isOpen}
        onModalClose={onClose}
        {...{ tableColumns, setTableData }}
      />
    </>
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

//TODO: add validation for the hazards against the backend
export const HazardsValidation = ({ input, valid }) => {
  if (!input || !valid) return null;

  return input === valid ? (
    <Icon as={FaCheckCircle} color="green.500" fontSize="lg" />
  ) : (
    <Popover>
      <PopoverTrigger>
        <IconButton
          aria-label="warning"
          icon={
            <Icon as={FaExclamationCircle} color="orange.500" fontSize="lg" />
          }
          size="sm"
          variant="ghost"
        />
      </PopoverTrigger>
      <PopoverContent color="white" bg="orange.500">
        <PopoverArrow />
        <PopoverCloseButton />
        <PopoverBody fontWeight="bold" border="0">
          Incorrect hazard code
        </PopoverBody>
      </PopoverContent>
    </Popover>
  );
};
