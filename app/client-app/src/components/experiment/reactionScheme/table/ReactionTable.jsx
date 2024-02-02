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
import { useReactionTable } from "./useReactionTableData";
import { FaCheckCircle, FaExclamationCircle, FaPlus } from "react-icons/fa";
import { AddSubstanceModal } from "../modal/AddSubstanceModal";

export const REACTION_TABLE_DEFAULT_VALUES = {
  tableData: [],
  massUnit: "",
};

export const ReactionTable = ({ name, ketcherData, isDisabled }) => {
  const { tableData, setTableData, massUnit, setMassUnit } = useReactionTable(
    name,
    ketcherData
  );

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

  return (
    <VStack align="flex-start">
      <DataTable
        data={tableData}
        setTableData={setTableData}
        columns={tableColumns}
        FooterCellAddRow={!isDisabled && <FooterCell {...{ setTableData }} />}
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
  const Reagent = useDisclosure();
  const Solvent = useDisclosure();

  return (
    <HStack justify="flex-end" spacing={5}>
      <Button
        colorScheme="pink"
        size="sm"
        leftIcon={<FaPlus />}
        onClick={Reagent.onOpen}
      >
        Add reagent
      </Button>
      <Button
        colorScheme="teal"
        size="sm"
        leftIcon={<FaPlus />}
        onClick={Solvent.onOpen}
      >
        Add solvent
      </Button>
      <AddSubstanceModal
        isModalOpen={Reagent.isOpen}
        onModalClose={Reagent.onClose}
        {...{ tableColumns, setTableData }}
      />
      <AddSubstanceModal
        isModalOpen={Solvent.isOpen}
        onModalClose={Solvent.onClose}
        {...{ tableColumns, setTableData }}
        isAddingSolvent
      />
    </HStack>
  );
};

const ColumnUnitHeaderDropdown = ({ options, value, setValue, isDisabled }) => (
  <Select
    minW="80px"
    size="xs"
    value={value}
    onChange={(e) => setValue(e.target.value)}
    isDisabled={isDisabled}
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
